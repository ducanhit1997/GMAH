using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GMAH.Services.Services
{
    public class BaseService
    {
        protected GMAHEntities _db;

        public BaseService()
        {
            _db = new GMAHEntities();
        }

        /// <summary>
        /// Convert entity thành view model
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected UserViewModel ConvertToViewModel(USER data)
        {
            if (data is null) return null;

            switch (data.IdRole)
            {
                case (int)RoleEnum.MANAGER:
                case (int)RoleEnum.ASSISTANT:
                case (int)RoleEnum.PARENT:
                    return new UserViewModel
                    {
                        IdRole = data.IdRole,
                        IdUser = data.IdUser,
                        Username = data.Username,
                        Fullname = data.Fullname,
                        Phone = data.Phone,
                        Email = data.Email,
                        Role = data.ROLE.RoleName,
                        CitizenID = data.CitizenId,
                        Address = data.Address ?? string.Empty,
                    };

                case (int)RoleEnum.TEACHER:
                case (int)RoleEnum.HEAD_OF_SUBJECT:
                    return new TeacherViewModel
                    {
                        IdRole = data.IdRole,
                        IdUser = data.IdUser,
                        Username = data.Username,
                        Fullname = data.Fullname,
                        Phone = data.Phone,
                        Email = data.Email,
                        Role = data.ROLE.RoleName,
                        CitizenID = data.CitizenId,
                        IdTeacher = data.TEACHERs.FirstOrDefault()?.IdTeacher ?? 0,
                        TeacherCode = data.TEACHERs.FirstOrDefault()?.TeacherCode ?? string.Empty,
                        Address = data.Address ?? string.Empty,
                    };

                case (int)RoleEnum.STUDENT:
                    return new StudentViewModel
                    {
                        IdRole = data.IdRole,
                        IdUser = data.IdUser,
                        Username = data.Username,
                        Fullname = data.Fullname,
                        Phone = data.Phone,
                        Email = data.Email,
                        Role = data.ROLE.RoleName,
                        CitizenID = data.CitizenId,
                        StudentCode = data.STUDENTs.FirstOrDefault()?.StudentCode ?? string.Empty,
                        Address = data.Address ?? string.Empty,
                    };
            }

            return null;
        }

        protected SemesterViewModel ConvertToViewModel(SEMESTER data)
        {
            if (data is null) return null;

            return new SemesterViewModel
            {
                IdSemester = data.IdSemester,
                SemesterName = data.SemesterName,
                SemesterYear = data.YEAR.YearName,
                ScoreWeight = data.ScoreWeight,
                IsCurrentSemester = data.IsCurrentSemester ?? false,
                DateEnd = data.DateEnd,
                DateStart = data.DateStart,
                IsYear = false,
            };
        }

        protected SemesterViewModel ConvertToViewModel(YEAR data)
        {
            if (data is null) return null;

            return new SemesterViewModel
            {
                IdSemester = data.IdYear,
                SemesterName = data.YearName,
                SemesterYear = data.YearName,
                IsYear = true,
            };
        }

        protected SubjectViewModel ConvertToViewModel(SUBJECT data)
        {
            if (data is null) return null;

            return new SubjectViewModel
            {
                IdSubject = data.IdSubject,
                SubjectName = data.SubjectName,
                SubjectCode = data.SubjectCode,
            };
        }

        protected ClassViewModel ConvertToViewModel(CLASS data)
        {
            if (data is null) return null;

            return new ClassViewModel
            {
                IdClass = data.IdClass,
                IdYear = data.IdYear,
                ClassName = data.ClassName,
                YearName = data.YEAR.YearName,
                IdFormTeacher = data.IdFormTeacher,
                FormTeacherFullname = data.TEACHER?.USER?.Fullname ?? string.Empty,
                Subject = data.CLASS_SUBJECT.Select(x => ConvertToViewModel(x)).ToList(),
            };
        }

        protected ClassSubjectTeacherViewModel ConvertToViewModel(CLASS_SUBJECT data)
        {
            if (data is null) return null;
            return new ClassSubjectTeacherViewModel
            {
                IdSubject = data.IdSubject,
                IdTeacherSubject = data.IdTeacherSubject,
            };
        }

        protected SettingViewModel ConvertToViewModel(SYSTEMSETTING data)
        {
            if (data is null) return null;

            return new SettingViewModel
            {
                Key = data.SettingKey,
                Name = data.SettingName,
                Value = data.SettingValue,
                Type = (InputTypeEnum)int.Parse(data.InputType),
            };
        }

        protected ScoreTypeViewModel ConvertToViewModel(SCORE_TYPE data)
        {
            if (data is null) return null;
            return new ScoreTypeViewModel
            {
                IdScoreType = data.IdScoreType,
                IdClass = data.CLASS_SUBJECT.IdClass,
                IdSubject = data.CLASS_SUBJECT.IdSubject,
                ScoreWeight = data.ScoreWeight ?? 0,
                ScoreName = data.ScoreName,
            };
        }

        protected List<GradeRuleViewModel> ConvertToViewModel(GRADERULE data)
        {
            if (data is null) return null;

            var vm = new List<GradeRuleViewModel>();

            foreach (var gradeRule in data.GRADERULELISTs)
            {
                var gradeRuleVM = new GradeRuleViewModel
                {
                    IdRank = (RankEnum)gradeRule.IdRank,
                    IdClass = data.CLASSes.Select(x => x.IdClass).ToList(),
                    ClassName = string.Join(", ", data.CLASSes.Select(x => x.ClassName).ToList()),
                    IdRule = gradeRule.IdRule,
                    IdSemester = data.IdSemester,
                    IdRuleList = gradeRule.IdRuleList,
                    MinAvgScore = gradeRule.MinAvgScore ?? 0,
                    Details = new List<RuleDetailViewModel>(),
                    IdBehaviour = gradeRule.IdBehaviour,
                    BehaviourName = gradeRule.IdBehaviour.HasValue ? ((BehaviourEnum)gradeRule.IdBehaviour.Value).GetAttribute<DisplayAttribute>().Name : "",
                };

                // Gán tên cho xếp hạng
                gradeRuleVM.GradeName = gradeRuleVM.IdRank.GetAttribute<DisplayAttribute>().Name;

                foreach (var detail in gradeRule.GRADERULEDETAILs)
                {
                    gradeRuleVM.Details.Add(new RuleDetailViewModel
                    {
                        IdRuleDetail = detail.IdRuleDetail,
                        IdSubject = detail.IdSubject,
                        SubjectName = detail.SUBJECT?.SubjectName ?? "Tất cả môn học",
                        MinAvgScore = detail.MinAvgScore,                        
                    });
                }

                vm.Add(gradeRuleVM);
            }

            return vm;
        }

        protected ReportViewModel ConvertToViewModel(REPORT data)
        {
            if (data is null) return null;

            var vm = new ReportViewModel
            {
                IdReport = data.IdReport,
                ReportContent = data.ReportContent,
                ReportTitle = data.ReportTitle,
                ReportStatus = (ReportStatusEnum)data.ReportStatus,
                ReportStatusName = data.REPORT_STATUS.StatusName,
                ReportType = (ReportTypeEnum)data.ReportType,
                FullnameSubmitReport = data.USER.Fullname,
                SubmitDate = data.SubmitDate,
                LastUpdateDate = data.LastUpdateDate,
                SubmitForIdUser = data.SubmitForIdUser,
                FullnameStudent = data.STUDENTUSER.Fullname,
                History = new List<ReviewViewModel>(),
                Files = new List<string>(),
            };

            foreach (var history in data.REPORT_HISTORY)
            {
                vm.History.Add(ConvertToViewModel(history));
            }

            foreach (var reportFile in data.REPORT_FILE)
            {
                vm.Files.Add(reportFile.Filename);
            }

            return vm;
        }

        protected ReviewViewModel ConvertToViewModel(REPORT_HISTORY data)
        {
            if (data is null) return null;
            return new ReviewViewModel
            {
                IdReportHistory = data.IdReportHistory,
                IdUserUpdate = data.IdUserUpdate,
                FullnameUserUpdate = data.USER.Fullname,
                ReportStatusName = data.REPORT_STATUS.StatusName,
                HistoryDate = data.HistoryDate,
                ReportStatus = (ReportStatusEnum)data.ReportStatus,
                Comment = data.Comment,
            };
        }
    }
}
