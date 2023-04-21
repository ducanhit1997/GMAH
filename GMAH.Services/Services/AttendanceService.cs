using GMAH.Entities;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    /// <summary>
    /// Điểm danh
    /// </summary>
    public class AttendanceService : BaseService
    {
        public BaseResponse GetClassAttendance(int idClass, DateTime date)
        {
            var classDB = _db.CLASSes.AsNoTracking().Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB == null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            // Lấy danh sách học sinh
            var studentInClass = classDB.STUDENT_CLASS.ToList();

            // Lấy toàn bộ data điểm danh
            var allAttendanceDateDB = studentInClass.SelectMany(x => x.ATTENDANCEs).Select(x => x.DateAttendance).Distinct().ToList();

            var vm = new ClassAttendanceViewModel
            {
                IdClass = idClass,
                AttendanceDate = date.Date,
                Students = new List<StudentAttendanceViewModel>(),
            };

            foreach (var student in studentInClass)
            {
                var studentVM = new StudentAttendanceViewModel
                {
                    IdStudent = student.STUDENT.IdUser,
                    Fullname = student.STUDENT.USER.Fullname,
                    StudentCode = student.STUDENT.StudentCode,
                    AttendanceDate = date.Date,
                    AttendanceStatus = AttendanceStatus.DID_NOT_ATTEND,
                };

                var attendanceDB = student.ATTENDANCEs.Where(x => x.DateAttendance.HasValue && x.DateAttendance.Value.Date == date.Date).FirstOrDefault();
                if (attendanceDB != null)
                {
                    vm.AssistantName = attendanceDB.USER?.Fullname;
                    vm.AssistantID = attendanceDB.AssistantID;
                    studentVM.AssistantName = attendanceDB.USER?.Fullname;
                    studentVM.AssistantID = attendanceDB.AssistantID;

                    if (attendanceDB.IsAvailable == true) studentVM.AttendanceStatus = AttendanceStatus.ATTENDED;
                    else if (attendanceDB.IsLeavePermission == true) studentVM.AttendanceStatus = AttendanceStatus.LEAVE_PERMISSION;
                }
                else
                {
                    studentVM.AttendanceStatus = AttendanceStatus.NONE;
                }

                vm.Students.Add(studentVM);
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = vm,
            };
        }

        public BaseResponse GetStudentAttendance(int idClass, List<int> idStudents, DateTime from, DateTime to)
        {
            if ((to - from).Days < 0 || (to - from).Days >= 90)
            {
                return new BaseResponse("Chỉ được xem dữ liệu trong 90 ngày");
            }

            var vm = new ListAttendanceViewModel
            {
                Headers = new List<string>(),
                Students = new List<StudentViewModel>(),
                StudentData = new List<StudentAttendanceViewModel>(),
            };

            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();

            if (classDB == null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }
            vm.Students = classDB.STUDENT_CLASS.Where(x => idStudents.Any(i => i == x.STUDENT.IdUser)).Select(x => ConvertToViewModel(x.STUDENT.USER) as StudentViewModel).ToList();

            // Student ID in class
            var studentIDsDB = classDB.STUDENT_CLASS.Select(x => x.STUDENT.IdUser).ToList();

            // Check student có trong lớp hay ko
            if (idStudents.Any(x => !studentIDsDB.Any(i => i == x)))
            {
                return new BaseResponse("Một số học sinh không tồn tại trong lớp, vui lòng tải lại trang");
            }

            var studentInClass = _db.STUDENT_CLASS.AsNoTracking().Where(x => x.IdClass == idClass && idStudents.Any(i => i == x.STUDENT.IdUser)).ToList();
            if (studentInClass == null)
            {
                return new BaseResponse("Không tìm thấy học sinh trong lớp học này");
            }

            // Lấy toàn bộ data điểm danh
            var allAttendanceDateDB = _db.STUDENT_CLASS.Where(x => x.IdClass == idClass).SelectMany(x => x.ATTENDANCEs).Select(x => x.DateAttendance).Distinct().ToList();

            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                vm.Headers.Add(date.ToString("dd/MM"));

                foreach (var studentDB in studentInClass)
                {
                    var attendanceVM = new StudentAttendanceViewModel
                    {
                        Fullname = studentDB.STUDENT.USER.Fullname,
                        StudentCode = studentDB.STUDENT.StudentCode,
                        IdStudent = studentDB.STUDENT.IdUser,
                        AttendanceDate = date.Date,
                        AttendanceStatus = AttendanceStatus.DID_NOT_ATTEND,
                        DateString = date.ToString("dd/MM"),
                    };

                    if (!allAttendanceDateDB.Any(x => x.Value.Date == date.Date))
                    {
                        attendanceVM.AttendanceStatus = AttendanceStatus.NONE;
                    }
                    else
                    {
                        var attendanceDB = studentDB.ATTENDANCEs.Where(x => x.DateAttendance.HasValue && x.DateAttendance.Value.Date == date.Date).FirstOrDefault();
                        if (attendanceDB != null)
                        {
                            attendanceVM.AssistantName = attendanceDB.USER?.Fullname;
                            attendanceVM.AssistantID = attendanceDB.AssistantID;

                            if (attendanceDB.IsAvailable == true) attendanceVM.AttendanceStatus = AttendanceStatus.ATTENDED;
                            else if (attendanceDB.IsLeavePermission == true) attendanceVM.AttendanceStatus = AttendanceStatus.LEAVE_PERMISSION;
                        }
                        else
                        {
                            attendanceVM.AttendanceStatus = AttendanceStatus.NONE;
                        }
                    }

                    vm.StudentData.Add(attendanceVM);
                }
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = vm,
            };
        }

        public BaseResponse SaveClassAttendance(ClassAttendanceViewModel data)
        {
            var classDB = _db.CLASSes.Where(x => x.IdClass == data.IdClass).FirstOrDefault();
            if (classDB == null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            // Student ID in data vm
            var studentIDsVM = data.Students.Select(x => x.IdStudent).ToList();

            // Student ID in class
            var studentIDsDB = classDB.STUDENT_CLASS.Select(x => x.STUDENT.IdUser).ToList();

            // Check student có trong lớp hay ko
            if (studentIDsVM.Any(x => !studentIDsDB.Any(i => i == x)))
            {
                return new BaseResponse("Một số học sinh không tồn tại trong lớp, vui lòng tải lại trang");
            }

            // Save data
            foreach (var studentVM in data.Students)
            {
                var studentInClassDB = classDB.STUDENT_CLASS.Where(x => x.STUDENT.IdUser == studentVM.IdStudent).FirstOrDefault();
                var attendanceDB = studentInClassDB.ATTENDANCEs.Where(x => x.DateAttendance.HasValue && x.DateAttendance.Value.Date == studentVM.AttendanceDate.Date).FirstOrDefault();

                if (attendanceDB is null)
                {
                    attendanceDB = new ATTENDANCE
                    {
                        IdStudentClass = studentInClassDB.IdStudentClass,
                        DateAttendance = studentVM.AttendanceDate,
                        AssistantID = data.AssistantID,
                        CheckinTime = DateTime.Now,
                    };

                    _db.ATTENDANCEs.Add(attendanceDB);
                }

                switch (studentVM.AttendanceStatus)
                {
                    case AttendanceStatus.ATTENDED:
                        attendanceDB.IsAvailable = true;
                        attendanceDB.IsLeavePermission = false;
                        break;
                    case AttendanceStatus.LEAVE_PERMISSION:
                        attendanceDB.IsAvailable = false;
                        attendanceDB.IsLeavePermission = true;
                        break;
                    case AttendanceStatus.DID_NOT_ATTEND:
                        attendanceDB.IsAvailable = false;
                        attendanceDB.IsLeavePermission = false;
                        break;
                }
            }

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

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
    }
}
