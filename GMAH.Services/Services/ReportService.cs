using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GMAH.Services.Services
{
    public class ReportService : BaseService
    {
        public ReportResponse GetReport(int idReport, int idUser)
        {
            var userDB = _db.USERs.Where(x => x.IdUser == idUser).FirstOrDefault();
            bool isAdmin = false;
            bool isReview = IsHavePermissionInReport(idUser, idReport);
            if (userDB.IdRole == (int)RoleEnum.MANAGER || userDB.IdRole == (int)RoleEnum.ASSISTANT)
            {
                isAdmin = true;
            }
            else if (userDB.IdRole == (int)RoleEnum.TEACHER || userDB.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT)
            {
                isAdmin = isReview;
            }

            var reportDB = _db.REPORTs.Where(x => x.IdReport == idReport && (isAdmin || x.IdUserSubmitReport == idUser || x.SubmitForIdUser == idUser)).FirstOrDefault();
            if (reportDB == null)
            {
                return new ReportResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy báo cáo này hoặc bạn không có quyền xem nó",
                };
            }

            var reportVM = ConvertToViewModel(reportDB);
            if (reportVM.ReportType == ReportTypeEnum.SCORE)
            {
                var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditScore>(reportDB.EditField);
                var editValue = double.Parse(reportDB.EditValue);

                var scoreDB = _db.SCORE_TYPE.AsNoTracking().Where(x => x.IdScoreType == fieldData.IdScoreType).FirstOrDefault();
                var semesterDB = _db.SEMESTERs.AsNoTracking().Where(x => x.IdSemester == fieldData.IdSemester).FirstOrDefault();
                var subjectDB = _db.SUBJECTs.AsNoTracking().Where(x => x.IdSubject == fieldData.IdSubject).FirstOrDefault();

                reportVM.Issue = $"Chỉnh sửa điểm số ở học kỳ {semesterDB?.SemesterName}, năm học {semesterDB?.YEAR.YearName}, môn học {subjectDB?.SubjectName}, cột điểm {scoreDB?.ScoreName}, điểm số yêu cầu chỉnh sửa là: {editValue}";
            }
            else
            {
                var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditAttendance>(reportDB.EditField);
                var editValue = int.Parse(reportDB.EditValue);

                var semesterDB = _db.SEMESTERs.AsNoTracking().Where(x => x.IdSemester == fieldData.IdSemester).FirstOrDefault();
                reportVM.Issue = $"Chỉnh sửa điểm danh vào ngày {fieldData.Date.ToString("dd/MM/yyyy")} thành {(editValue == 0 ? "Có mặt" : "Vắng có phép")}";
            }

            return new ReportResponse
            {
                IsSuccess = true,
                IsReview = isReview,
                Object = reportVM,
            };
        }

        /// <summary>
        /// Lấy toàn bộ report
        /// </summary>
        public PaginationResponse GetListReport(int? idUserSubmit, int? status, DatatableParam filter)
        {
            var listReportDB = _db.REPORTs.Where(x => (idUserSubmit == null || x.IdUserSubmitReport == idUserSubmit || x.SubmitForIdUser == idUserSubmit) && (status == null || x.ReportStatus == status));

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listReportDB = listReportDB.Where(x => x.ReportTitle.Contains(value));
            }

            var data = listReportDB.OrderByDescending(x => x.SubmitDate).Skip(filter.start).Take(filter.length).ToList();

            var listVM = new List<ReportViewModel>();

            // Convert danh sách
            foreach (var report in data)
            {
                listVM.Add(ConvertToViewModel(report));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listReportDB.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Lấy các report cần xử lý
        /// </summary>
        public PaginationResponse GetMyReviewReport(int idUser, DatatableParam filter)
        {

            // Lấy user
            var userDB = _db.USERs.Where(x => x.IdUser == idUser).FirstOrDefault();
            var status = new List<int>() { -1 };
            switch ((RoleEnum)userDB.IdRole)
            {
                case RoleEnum.ASSISTANT:
                    status = new List<int>() { 2 };
                    break;
                case RoleEnum.TEACHER:
                case RoleEnum.HEAD_OF_SUBJECT:
                    status = new List<int>() { 1, 3 };
                    break;
            }

            var listReportDB = _db.REPORTs.Where(x => status.Any(i => i == x.ReportStatus)).ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listReportDB = listReportDB.Where(x => x.ReportTitle.Contains(value)).ToList();
            }

            var listReportFilter = listReportDB;

            // Nếu là gv thì phải là gvcn của lớp đó
            if ((RoleEnum)userDB.IdRole == RoleEnum.TEACHER || (RoleEnum)userDB.IdRole == RoleEnum.HEAD_OF_SUBJECT)
            {
                var idTeacher = userDB.TEACHERs.First().IdTeacher;
                listReportFilter = listReportDB.Where(x =>
                {
                    if (x.ReportStatus == (int)ReportStatusEnum.WAIT_FORM_TEACHER)
                    {
                        return x.STUDENTUSER.STUDENTs.Any(i => i.STUDENT_CLASS.Any(c => c.CLASS.IdFormTeacher == idTeacher));
                    }
                    else if (x.ReportStatus == (int)ReportStatusEnum.WAIT_TEACHER_SUBJECT)
                    {
                        var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditScore>(x.EditField);

                        var classDB = _db.CLASSes.AsNoTracking().Where(c => c.STUDENT_CLASS.Any(i => i.STUDENT.IdUser == x.SubmitForIdUser) && c.YEAR.SEMESTERs.Any(i => i.IdSemester == fieldData.IdSemester)).FirstOrDefault();
                        var classSubjectDB = classDB.CLASS_SUBJECT.Where(i => i.IdSubject == fieldData.IdSubject).FirstOrDefault();
                        if (classSubjectDB != null)
                        {
                            if (classSubjectDB.TEACHER_SUBJECT.IdTeacher == idTeacher)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }).ToList();
            }

            var data = listReportFilter.OrderByDescending(x => x.SubmitDate).Skip(filter.start).Take(filter.length).ToList();

            var listVM = new List<ReportViewModel>();

            // Convert danh sách
            foreach (var report in data)
            {
                listVM.Add(ConvertToViewModel(report));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listReportFilter.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Gửi report lên trên
        /// </summary>
        public BaseResponse SubmitReport(ReportViewModel data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new BaseResponse(validateModel.ErrorMessage);
            }

            var reportDB = new REPORT
            {
                IdUserSubmitReport = data.IdUserSubmitReport,
                SubmitForIdUser = data.SubmitForIdUser,
                LastUpdateDate = DateTime.Now,
                SubmitDate = DateTime.Now,
                ReportTitle = data.ReportTitle,
                ReportContent = data.ReportContent,
                ReportStatus = 1,
                ReportType = (int)data.ReportType,
                EditField = data.EditField,
                EditValue = data.EditValue,
            };

            foreach (var reportFile in data.Files)
            {
                reportDB.REPORT_FILE.Add(new REPORT_FILE
                {
                    Filename = reportFile
                });
            }

            _db.REPORTs.Add(reportDB);

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Thành công
                return new BaseResponse
                {
                    Object = reportDB.IdReport,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }

        /// <summary>
        /// Review (approve hoặc reject) report
        /// </summary>
        public BaseResponse ReviewReport(ReviewViewModel data)
        {
            if (!IsHavePermissionInReport(data.IdUserUpdate, data.IdReport))
            {
                return new BaseResponse("Bạn không có quyền duyệt báo cáo này");
            }

            var reportDB = _db.REPORTs.Where(x => x.IdReport == data.IdReport).FirstOrDefault();
            if (reportDB is null)
            {
                return new BaseResponse("Không tìm thấy báo cáo này");
            }

            if (reportDB.ReportStatus == (int)ReportStatusEnum.APPROVE || reportDB.ReportStatus == (int)ReportStatusEnum.REJECT)
            {
                return new BaseResponse("Báo cáo này đã hoàn tất, không thể cập nhật thêm dữ liệu");
            }

            reportDB.LastUpdateDate = DateTime.Now;
            if (data.ReportStatus == ReportStatusEnum.REJECT)
            {
                reportDB.ReportStatus = (int)ReportStatusEnum.REJECT;
            }
            else
            {
                // Gửi report lên trên
                var newStatus = reportDB.ReportStatus + 1;
                switch ((ReportTypeEnum)reportDB.ReportType)
                {
                    case ReportTypeEnum.SCORE:
                        if (newStatus > (int)ReportStatusEnum.WAIT_TEACHER_SUBJECT)
                        {
                            newStatus = (int)ReportStatusEnum.APPROVE;
                        }
                        else if (newStatus == (int)ReportStatusEnum.WAIT_ASSISTANT)
                        {
                            newStatus = (int)ReportStatusEnum.WAIT_TEACHER_SUBJECT;
                        }

                        break;
                    case ReportTypeEnum.ATTENDANCE:
                        if (newStatus > (int)ReportStatusEnum.WAIT_ASSISTANT)
                        {
                            newStatus = (int)ReportStatusEnum.APPROVE;
                        }

                        break;
                }

                if (newStatus == (int)ReportStatusEnum.APPROVE)
                {
                    ApproveReport(data.IdUserUpdate, reportDB);
                }

                reportDB.ReportStatus = newStatus;
            }

            // Tạo history
            var reportHistoryDB = new REPORT_HISTORY
            {
                IdReport = data.IdReport,
                Comment = data.Comment,
                ReportStatus = (int)data.ReportStatus,
                IdUserUpdate = data.IdUserUpdate,
                HistoryDate = DateTime.Now,
            };

            _db.REPORT_HISTORY.Add(reportHistoryDB);

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Kiểm tra list mail của phụ huynh
                var listEmail = reportDB.STUDENTUSER.STUDENTs.First().USERs.Select(x => x.Email).ToList();
                if (listEmail.Any(x => string.IsNullOrEmpty(x)))
                {
                    return new BaseResponse
                    {
                        IsSuccess = true,
                    };
                }

                // Gửi mail duyệt thành công
                // Gửi mail bằng bg
                // Lấy thông tin email
                var settingService = new SystemSettingService();
                var emailUsername = settingService.GetSetting(SettingEnum.EMAIL_USERNAME);
                var emailPassword = settingService.GetSetting(SettingEnum.EMAIL_PASSWORD);
                var senderName = settingService.GetSetting(SettingEnum.EMAIL_SENDERNAME);
                var smtp = settingService.GetSetting(SettingEnum.EMAIL_SMTP);
                var port = settingService.GetSetting(SettingEnum.EMAIL_PORT);

                // Nếu setting ko có info thì báo lỗi
                if (string.IsNullOrEmpty(emailUsername) ||
                    string.IsNullOrEmpty(emailPassword) ||
                    string.IsNullOrEmpty(senderName) ||
                    string.IsNullOrEmpty(smtp) ||
                    string.IsNullOrEmpty(port))
                {
                    return new BaseResponse
                    {
                        IsSuccess = true,
                    };
                }

                var emailTemplate = settingService.GetSetting(SettingEnum.EMAIL_TEMPLATE_REPORT) ?? String.Empty;
                emailTemplate = emailTemplate.Replace("{idreport}", reportDB.IdReport.ToString());
                emailTemplate = emailTemplate.Replace("{reporttitle}", reportDB.ReportTitle);
                emailTemplate = emailTemplate.Replace("{status}", reportDB.REPORT_STATUS.StatusName);

                var emailUtility = new EmailUtility(senderName, emailUsername, emailPassword, smtp, port);

                try
                {
                    int idReport = reportDB.IdReport;
                    Thread trd = new Thread(new ThreadStart(() =>
                    {
                        emailUtility.Send(listEmail, $"BÁO CÁO SỐ " + idReport + " CÓ CẬP NHẬT MỚI", emailTemplate);
                    }));
                    trd.IsBackground = true;
                    trd.Start();
                }
                catch
                {
                    // Do nothing
                }

                // Thành công
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }

        public bool IsHavePermissionInReport(int idUser, int idReport)
        {
            var userDB = _db.USERs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (userDB == null) return false;

            var status = new List<int>() { -1 };
            switch ((RoleEnum)userDB.IdRole)
            {
                case RoleEnum.ASSISTANT:
                    status = new List<int>() { 2 };
                    break;
                case RoleEnum.TEACHER:
                case RoleEnum.HEAD_OF_SUBJECT:
                    status = new List<int>() { 1, 3 };
                    break;
            }

            var reportDB = _db.REPORTs.Where(x => x.IdReport == idReport && status.Any(i => i == x.ReportStatus)).FirstOrDefault();
            if (reportDB == null) return false;

            // Nếu là gv thì phải là gvcn của lớp đó
            if ((RoleEnum)userDB.IdRole == RoleEnum.TEACHER || (RoleEnum)userDB.IdRole == RoleEnum.HEAD_OF_SUBJECT)
            {
                var idTeacher = userDB.TEACHERs.First().IdTeacher;
                if (reportDB.ReportStatus == (int)ReportStatusEnum.WAIT_FORM_TEACHER)
                {
                    if (reportDB.STUDENTUSER.STUDENTs.Any(i => i.STUDENT_CLASS.Any(c => c.CLASS.IdFormTeacher == idTeacher)))
                    {
                        return true;
                    }
                }
                else if (reportDB.ReportStatus == (int)ReportStatusEnum.WAIT_TEACHER_SUBJECT)
                {
                    var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditScore>(reportDB.EditField);
                    var classDB = _db.CLASSes.AsNoTracking().Where(c => c.STUDENT_CLASS.Any(i => i.STUDENT.IdUser == reportDB.SubmitForIdUser) && c.YEAR.SEMESTERs.Any(i => i.IdSemester == fieldData.IdSemester)).FirstOrDefault();
                    var classSubjectDB = classDB.CLASS_SUBJECT.Where(i => i.IdSubject == fieldData.IdSubject).FirstOrDefault();
                    if (classSubjectDB != null)
                    {
                        if (classSubjectDB.TEACHER_SUBJECT.IdTeacher == idTeacher)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        private void ApproveReport(int idAdmin, REPORT reportDB)
        {
            switch ((ReportTypeEnum)reportDB.ReportType)
            {
                case ReportTypeEnum.SCORE:
                    ApproveScoreReport(idAdmin, reportDB);
                    break;
                case ReportTypeEnum.ATTENDANCE:
                    ApproveAttendanceReport(idAdmin, reportDB);
                    break;
            }
        }

        private void ApproveScoreReport(int idAdmin, REPORT reportDB)
        {
            var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditScore>(reportDB.EditField);
            var editValue = double.Parse(reportDB.EditValue);

            var scoreService = new ScoreService();
            scoreService.AddStudentScore(idAdmin, reportDB.SubmitForIdUser, fieldData.IdSubject, fieldData.IdSemester, fieldData.IdScoreType, editValue, "Chỉnh sửa điểm dựa trên báo cáo số " + reportDB.IdReport);
        }

        private void ApproveAttendanceReport(int idAdmin, REPORT reportDB)
        {
            var fieldData = JsonConvert.DeserializeObject<ReportEditFieldData.EditAttendance>(reportDB.EditField);
            var editValue = int.Parse(reportDB.EditValue);
            fieldData.IdClass = (new ClassService()).GetStudentClassInSemester(reportDB.SubmitForIdUser, fieldData.IdSemester) ?? 0;

            var scoreService = new AttendanceService();
            var dataVM = new ClassAttendanceViewModel
            {
                IdClass = fieldData.IdClass,
                AttendanceDate = fieldData.Date,
                AssistantID = idAdmin,
                Students = new List<StudentAttendanceViewModel> {
                    new StudentAttendanceViewModel
                    {
                        IdStudent = reportDB.SubmitForIdUser,
                        AssistantID = idAdmin,
                        AttendanceDate = fieldData.Date,
                        AttendanceStatus = (AttendanceStatus)editValue,
                    }
                },
            };
            scoreService.SaveClassAttendance(dataVM);
        }
    }
}
