using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    public class ClassService : BaseService
    {
        /// <summary>
        /// Lấy danh sách lớp theo học kỳ
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaginationResponse PaginationClassBySemester(int idYear, DatatableParam filter)
        {
            // Lấy danh sách lớp theo học kỳ
            var listClass = _db.CLASSes.AsNoTracking()
                .Where(x => x.IdYear == idYear)
                .OrderByDescending(x => x.ClassName)
                .ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listClass = listClass.Where(x => x.ClassName.Contains(value)).ToList();
            }

            // Phân trang
            var data = listClass.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách
            var listVM = new List<ClassViewModel>();
            foreach (var _class in data)
            {
                listVM.Add(ConvertToViewModel(_class));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listClass.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Lấy thông tin một lớp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse GetClassById(int id)
        {
            var classDB = _db.CLASSes.AsNoTracking().Where(x => x.IdClass == id).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (classDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không tồn tại",
                };
            }

            // Trả về user
            return new BaseResponse
            {
                IsSuccess = true,
                Object = ConvertToViewModel(classDB),
            };
        }

        /// <summary>
        /// Lưu hoặc tạo mới lớp
        /// </summary>
        /// <param name="data"></param>
        public BaseResponse SaveClass(ClassViewModel data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == data.IdClass).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                if (data.IdClass > 0)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy dữ liệu này",
                    };
                }
                else
                {
                    // Tạo mới
                    classDB = new CLASS();
                    _db.CLASSes.Add(classDB);
                }
            }

            // Thay đổi dữ liệu
            classDB.ClassName = data.ClassName;
            classDB.IdYear = data.IdYear.Value;

            // Kiểm tra teacher có đang là GVCN ở 1 lớp khác ko
            if (_db.CLASSes.Any(x => x.IdFormTeacher == data.IdFormTeacher && x.IdYear == data.IdYear.Value && x.IdClass != data.IdClass))
            {
                return new BaseResponse("Giáo viên này đang chủ nhiệm một lớp khác trong cùng năm học, 1 giáo viên chỉ được chủ nhiệm 1 lớp trong 1 năm học");
            }

            classDB.IdFormTeacher = data.IdFormTeacher;

            // Nạp môn học
            var currentIdSubject = classDB.CLASS_SUBJECT.Select(x => x.IdSubject).ToList();
            var listIdSubject = data.Subject?.Select(x => x.IdSubject).ToList() ?? new List<int>();
            var listNeedRemove = currentIdSubject.Where(x => !listIdSubject.Any(i => i == x)).ToList();
            var listNeedAdd = listIdSubject.Where(x => !currentIdSubject.Any(i => i == x)).ToList();
            var listNeedUpdate = listIdSubject.Where(x => currentIdSubject.Any(i => i == x)).ToList();

            // Add new
            foreach (var idAdd in listNeedAdd)
            {
                var dataSubject = data.Subject.Where(x => x.IdSubject == idAdd).FirstOrDefault();

                classDB.CLASS_SUBJECT.Add(new CLASS_SUBJECT
                {
                    IdSubject = idAdd,
                    IdTeacherSubject = dataSubject?.IdTeacherSubject,
                });
            }

            // Remove
            foreach (var idRemove in listNeedRemove)
            {
                var removeObj = classDB.CLASS_SUBJECT.Where(x => x.IdSubject == idRemove).First();
                _db.CLASS_SUBJECT.Remove(removeObj);
            }

            // Update
            foreach (var idUpdate in listNeedUpdate)
            {
                var dataSubject = data.Subject.Where(x => x.IdSubject == idUpdate).FirstOrDefault();
                var classSubjectDB = classDB.CLASS_SUBJECT.Where(x => x.IdSubject == idUpdate).FirstOrDefault();

                classSubjectDB.IdTeacherSubject = dataSubject.IdTeacherSubject;
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
                    Object = classDB.IdClass,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Xoá lớp học
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse DeleteClass(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == id).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Hard delete
            _db.TIMELINEs.RemoveRange(classDB.TIMELINEs.ToList());


            // Xoá điểm
            foreach (var studentClass in classDB.STUDENT_CLASS.ToList())
            {
                _db.ATTENDANCEs.RemoveRange(studentClass.ATTENDANCEs.ToList());
                _db.SEMESTERRANKs.RemoveRange(studentClass.SEMESTERRANKs.ToList());
                _db.SCOREs.RemoveRange(studentClass.SCOREs.ToList());
            }

            _db.STUDENT_CLASS.RemoveRange(classDB.STUDENT_CLASS.ToList());


            // Xoá môn học của lớp
            foreach (var classSubject in classDB.CLASS_SUBJECT.ToList())
            {
                _db.SCORE_TYPE.RemoveRange(classSubject.SCORE_TYPE.ToList());
            }
            _db.CLASS_SUBJECT.RemoveRange(classDB.CLASS_SUBJECT.ToList());


            _db.CLASSes.Remove(classDB);

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
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Thêm học sinh vào lớp học
        /// </summary>
        /// <param name="idUser"></param>
        public BaseResponse AddStudentToClass(int idClass, List<int> idUsers)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Trích xuất học kỳ
            var idYear = classDB.IdYear;

            // Lấy danh sách học sinh
            var studentsDB = _db.USERs
                .Where(x => x.IsDeleted != true && idUsers.Any(i => i == x.IdUser) && x.IdRole == (int)RoleEnum.STUDENT)
                .Select(x => x.STUDENTs.First())
                .ToList();

            // Kiểm tra số lượng học sinh
            if (studentsDB.Count != idUsers.Count)
            {
                return new BaseResponse("Danh sách học sinh không hợp lệ");
            }

            // Lấy danh sách id user của student
            var idStudents = studentsDB.Select(x => x.IdUser).ToList();

            // Thêm hoặc xoá dữ liệu
            var listCurrentStudentInClass = classDB.STUDENT_CLASS.Select(x => x.IdStudent).ToList();
            var listStudentNeedAddNew = idStudents.Where(x => !listCurrentStudentInClass.Any(i => i == x)).ToList();
            var listStudentNeedRemove = classDB.STUDENT_CLASS.Where(x => !idStudents.Any(i => i == x.IdStudent)).ToList();

            // Add new
            foreach (var addNew in listStudentNeedAddNew)
            {
                // Kiểm tra học sinh này có đang tham gia một lớp nào trong cùng học kỳ ko
                if (_db.STUDENT_CLASS.Any(x => x.IdStudent == addNew && x.CLASS.IdYear == idYear))
                {
                    var studentConfictDB = _db.STUDENTs.Where(x => x.IdStudent == addNew).FirstOrDefault();
                    return new BaseResponse($"Học sinh {studentConfictDB.USER.Fullname} đang tham gia một lớp khác trong cùng học kỳ");
                }

                classDB.STUDENT_CLASS.Add(new STUDENT_CLASS
                {
                    IdStudent = addNew,
                    IdClass = idClass,
                });
            }

            // Remove
            foreach (var remove in listStudentNeedRemove)
            {
                classDB.STUDENT_CLASS.Remove(remove);
            }

            try
            {
                // Lưu lại
                _db.SaveChanges();
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

        /// <summary>
        /// Lấy danh sách học sinh trong lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <returns></returns>
        public PaginationResponse GetStudentInClass(int idClass, DatatableParam filter)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                return new PaginationResponse();
            }

            // Lấy danh sách học sinh
            var listStudent = classDB.STUDENT_CLASS.Select(x => x.STUDENT).ToList().Select(x => x.USER).ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listStudent = listStudent.Where(x => x.Fullname.Contains(value)).ToList();
            }

            // Phân trang
            var data = listStudent.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách
            var listVM = new List<UserViewModel>();
            foreach (var _class in data)
            {
                listVM.Add(ConvertToViewModel(_class));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listStudent.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Lấy danh sách học sinh trong lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <returns></returns>
        public List<UserViewModel> GetStudentVMInClass(int idClass)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                return null;
            }

            // Lấy danh sách học sinh
            var listStudent = classDB.STUDENT_CLASS.Select(x => x.STUDENT).ToList().Select(x => x.USER).ToList();

            // Convert danh sách
            var listVM = new List<UserViewModel>();
            foreach (var _class in listStudent)
            {
                listVM.Add(ConvertToViewModel(_class));
            }

            return listVM;
        }

        /// <summary>
        /// Lấy danh sách lớp mà giáo viên đó được vào
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        public GetTeacherClassBySemesterResponse GetTeacherClassBySemester(int idUser, int? idSemester, int? idYear, bool viewScore = false)
        {
            // Kiểm tra user tồn tại không
            var userDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == idUser && x.IsDeleted != true).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (userDB is null)
            {
                return new GetTeacherClassBySemesterResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại",
                };
            }

            // Lấy danh sách lớp mà giáo viên đó được bổ nhiệm
            var idTeacher = userDB.TEACHERs.FirstOrDefault()?.IdTeacher ?? 0;
            var allClass = _db.CLASSes.Where(x => (idYear != null && x.IdYear == idYear) || (idSemester != null && x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)));

            // Phân quyền theo role
            switch (userDB.IdRole)
            {
                // Admin được hiển thị toàn bộ class
                case (int)RoleEnum.MANAGER:
                case (int)RoleEnum.ASSISTANT:
                    break;

                // Giáo viên hiển thị các lớp được bổ nhiệm
                case (int)RoleEnum.TEACHER:
                case (int)RoleEnum.HEAD_OF_SUBJECT:
                    if (!viewScore)
                    {
                        // Chế độ quản lý, chỉ hiển thị lớp đc quản lý
                        allClass = allClass.Where(x => x.IdFormTeacher == idTeacher);
                    }
                    else
                    {
                        // Chế độ xem, sửa điểm, cho phép hiển thị các lớp đang dạy
                        allClass = allClass.Where(x => x.IdFormTeacher == idTeacher || x.CLASS_SUBJECT.Any(c => c.TEACHER_SUBJECT.TEACHER.IdTeacher == idTeacher));
                    }
                    break;
                default:
                    // Không hiện class nào
                    allClass = allClass.Where(x => x.IdClass < 0);
                    break;
            }

            var listTeacherClass = allClass.ToList().Select(x => ConvertToViewModel(x)).ToList();
            return new GetTeacherClassBySemesterResponse
            {
                IsSuccess = true,
                Object = listTeacherClass,
                IdCurrentSemester = idSemester,
            };
        }

        /// <summary>
        /// Thêm một học sinh vào lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse AddStudentToClass(int idClass, int idUser)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }


            // Lấy danh sách học sinh
            var studentDB = _db.USERs
                .Where(x => x.IsDeleted != true && x.IdUser == idUser && x.IdRole == (int)RoleEnum.STUDENT)
                .SelectMany(x => x.STUDENTs)
                .FirstOrDefault();
            if (studentDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy học sinh này");
            }

            // Kiểm tra học sinh có trong lớp chưa
            if (studentDB.STUDENT_CLASS.Any(x => x.IdClass == idClass))
            {
                return new BaseResponse("Học sinh đã có trong lớp học rồi");
            }

            // Kiểm tra hs đang tham gia lớp khác
            if (studentDB.STUDENT_CLASS.Any(x => x.CLASS.IdYear == classDB.IdYear))
            {
                return new BaseResponse("Học sinh này đang tham gia một lớp khác, không thể thêm vào lớp hiện tại");
            }

            // Thêm vào lớp
            _db.STUDENT_CLASS.Add(new STUDENT_CLASS
            {
                IdClass = idClass,
                IdStudent = studentDB.IdStudent,
            });


            try
            {
                // Lưu lại
                _db.SaveChanges();
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

        /// <summary>
        /// Xoá 1 học sinh ra khỏi lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public BaseResponse RemoveStudentToClass(int idClass, int idUser)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }


            // Lấy danh sách học sinh
            var studentDB = _db.USERs
                .Where(x => x.IsDeleted != true && x.IdUser == idUser && x.IdRole == (int)RoleEnum.STUDENT)
                .SelectMany(x => x.STUDENTs)
                .FirstOrDefault();
            if (studentDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy học sinh này");
            }

            // Kiểm tra học sinh có trong lớp chưa
            var studentClassDB = studentDB.STUDENT_CLASS.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (studentClassDB is null)
            {
                return new BaseResponse("Học sinh chưa có trong lớp học này");
            }

            // Thêm vào lớp
            _db.STUDENT_CLASS.Remove(studentClassDB);

            try
            {
                // Lưu lại
                _db.SaveChanges();
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

        /// <summary>
        /// Kiểm tra user có quyền hạn trên class hay ko
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idClass"></param>
        /// <returns></returns>
        public bool IsUserHavePermissionInClass(int idUser, int idClass)
        {
            // Kiểm tra dữ liệu có tồn tại ko
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                // Báo lỗi tìm ko thấy
                return false;
            }

            var userDB = _db.USERs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (userDB is null)
            {
                // Báo lỗi tìm ko thấy
                return false;
            }

            // Nếu user là admin thì có quyền
            switch (userDB.IdRole)
            {
                // Admin được hiển thị toàn bộ class
                case (int)RoleEnum.MANAGER:
                case (int)RoleEnum.ASSISTANT:
                    return true;

                // Giáo viên hiển thị các lớp được bổ nhiệm
                case (int)RoleEnum.TEACHER:
                case (int)RoleEnum.HEAD_OF_SUBJECT:
                    return classDB.IdFormTeacher.HasValue && classDB.IdFormTeacher.Value == userDB.TEACHERs.First().IdTeacher;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Lấy danh sách môn học mà giáo viên dạy cho lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<SubjectViewModel> GetSubjectTeacherClass(int idClass, int userId)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();
            if (classDB is null)
            {
                return null;
            }

            // Lấy role user
            var userDB = _db.USERs.Where(x => x.IdUser == userId).FirstOrDefault();
            if (userDB is null) return new List<SubjectViewModel>();

            if (userDB.IdRole == (int)RoleEnum.MANAGER || userDB.IdRole == (int)RoleEnum.ASSISTANT)
            {
                var subjectDB = classDB.CLASS_SUBJECT.Select(x => x.SUBJECT).ToList();
                return subjectDB.Select(x => ConvertToViewModel(x)).ToList();
            }
            else
            {
                var subjectDB = classDB.CLASS_SUBJECT.Where(x => x.IdTeacherSubject != null && x.TEACHER_SUBJECT.TEACHER.IdUser == userId).Select(x => x.SUBJECT).ToList();
                return subjectDB.Select(x => ConvertToViewModel(x)).ToList();
            }
        }

        /// <summary>
        /// Lấy id class
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int? GetStudentClassInSemester(int idStudent, int idSemester)
        {
            var classDB = _db.CLASSes.Where(x => x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester) && x.STUDENT_CLASS.Any(i => i.STUDENT.IdUser == idStudent)).FirstOrDefault();
            return classDB?.IdClass;
        }

        /// <summary>
        /// Lấy id class
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int? GetStudentClassInYear(int idStudent, int idYear)
        {
            var classDB = _db.CLASSes.Where(x => x.IdYear == idYear && x.STUDENT_CLASS.Any(i => i.STUDENT.IdUser == idStudent)).FirstOrDefault();
            return classDB?.IdClass;
        }
    }
}
