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
    public class SubjectService : BaseService
    {
        /// <summary>
        /// Lấy danh sách subject có phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaginationResponse PaginationSubject(DatatableParam filter)
        {
            // Lấy danh sách subject
            var listSubject = _db.SUBJECTs.AsNoTracking().OrderByDescending(x => x.IdSubject).ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listSubject = listSubject.Where(x => x.SubjectName.Contains(value)).ToList();
            }

            // Phân trang
            var data = listSubject.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách theo role
            var listVM = new List<SubjectViewModel>();
            foreach (var subject in data)
            {
                listVM.Add(ConvertToViewModel(subject));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listSubject.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Tạo hoặc lưu subject
        /// </summary>
        /// <param name="data"></param>
        public BaseResponse SaveSubject(SubjectViewModel data)
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
            var subjectDB = _db.SUBJECTs.Where(x => x.IdSubject == data.IdSubject).FirstOrDefault();
            if (subjectDB is null)
            {
                // Báo lỗi tìm ko thấy
                if (data.IdSubject > 0)
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
                    subjectDB = new SUBJECT();
                    _db.SUBJECTs.Add(subjectDB);
                }
            }

            // Kiểm tra tên môn học
            if (_db.SUBJECTs.Any(x => x.SubjectName.Equals(data.SubjectName, StringComparison.OrdinalIgnoreCase) && x.IdSubject != data.IdSubject))
            {
                return new BaseResponse("Tên môn học này đã tồn tại trong hệ thống");
            }
            if (_db.SUBJECTs.Any(x => x.SubjectCode.Equals(data.SubjectCode, StringComparison.OrdinalIgnoreCase) && x.IdSubject != data.IdSubject))
            {
                return new BaseResponse("Mã môn học này đã tồn tại trong hệ thống");
            }

            // Thay đổi dữ liệu
            subjectDB.SubjectName = data.SubjectName;
            subjectDB.SubjectCode = data.SubjectCode;

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Thành công
                return new BaseResponse
                {
                    IsSuccess = true,
                    Object = subjectDB.IdSubject,
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
        /// Xoá subject bằng Id
        /// </summary>
        /// <param name="id"></param>
        public BaseResponse DeleteSubject(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var subjectDB = _db.SUBJECTs.Where(x => x.IdSubject == id).FirstOrDefault();
            if (subjectDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Không thể xoá nếu có giáo viên bên đang phụ trách môn học
            if (subjectDB.HEAD_OF_SUBJECT.Any() || subjectDB.TEACHER_SUBJECT.Any())
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Môn học này có giáo viên đang phụ trách, có thể là giáo viên bộ môn hoặc trưởng bộ môn");
            }

            // Hard delete
            _db.SUBJECTs.Remove(subjectDB);

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
        /// Lấy thông tin của 1 subject bằng id
        /// </summary>
        /// <param name="id"></param>
        public BaseResponse GetSubjectById(int id)
        {
            var subjectDB = _db.SUBJECTs.AsNoTracking().Where(x => x.IdSubject == id).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (subjectDB is null)
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
                Object = ConvertToViewModel(subjectDB),
            };
        }

        public BaseResponse GetClassSubject(int idClass)
        {
            var subjectDB = _db.CLASS_SUBJECT.Where(x => x.IdClass == idClass).Select(x => x.SUBJECT).ToList();

            // Báo lỗi nếu user ko tồn tại
            if (subjectDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không tồn tại",
                };
            }

            // Convert danh sách
            var listVM = new List<SubjectViewModel>();
            foreach (var subject in subjectDB)
            {
                listVM.Add(ConvertToViewModel(subject));
            }


            // Trả về user
            return new BaseResponse
            {
                IsSuccess = true,
                Object = listVM,
            };
        }

        /// <summary>
        /// Lấy danh sách giáo viên bộ môn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse GetTeacherInSubject(int id)
        {
            var result = new TeacherInSubjectViewModel
            {
                Teachers = new List<UserViewModel>(),
                HeadOfSubject = new List<UserViewModel>(),
                TeacherInCurrentSubject = new List<UserViewModel>(),
                HeadOfCurrentSubject = new List<UserViewModel>(),
            };

            // Nếu không có id thì return empty list
            if (id == 0)
            {
                return new BaseResponse
                {
                    IsSuccess = true,
                    Object = result,
                };
            }

            // Lấy toàn bộ giáo viện tồn tại trong system
            var allTeacher = _db.USERs.AsNoTracking().Where(x => x.IsDeleted != true &&
                                                 (x.IdRole == (int)RoleEnum.TEACHER ||
                                                  x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT)).ToList();

            // Danh sách giáo viên bộ môn
            var teachersInSubject = allTeacher.Where(x => x.TEACHERs.Any(t => t.TEACHER_SUBJECT.Any(s => s.IdSubject == id))).ToList();
            var teachersHeadOfSubject = allTeacher.Where(x => x.TEACHERs.Any(t => t.HEAD_OF_SUBJECT.Any(s => s.IdSubject == id))).ToList();

            // Gán vào result
            result.TeacherInCurrentSubject = teachersInSubject.Select(x => ConvertToViewModel(x)).ToList();
            result.HeadOfCurrentSubject = teachersHeadOfSubject.Select(x =>
            {
                var headSubject = ConvertToViewModel(x);
                var headSubjectDB = x.TEACHERs.SelectMany(i => i.HEAD_OF_SUBJECT).Where(i => i.IdSubject == id).FirstOrDefault();
                headSubject.FromYear = headSubjectDB?.FromYear;
                headSubject.ToYear = headSubjectDB?.ToYear;

                return headSubject;
            }).ToList();

            // Loại các ds ứng viên đã tồn tại bên trên
            result.HeadOfSubject = allTeacher
                .Where(x => x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT && !teachersHeadOfSubject.Any(t => t.IdUser == x.IdUser))
                .Select(x => ConvertToViewModel(x))
                .ToList();
            result.Teachers = allTeacher
                .Where(x => !teachersInSubject.Any(t => t.IdUser == x.IdUser))
                .Select(x => ConvertToViewModel(x))
                .ToList();

            return new BaseResponse
            {
                IsSuccess = true,
                Object = result,
            };
        }

        /// <summary>
        /// Thêm giáo viên ra vào dạnh sách giáo viên bộ môn
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public BaseResponse SetTeacherInSubject(int idUser, int idSubject)
        {
            // Kiểm tra giáo viên và bộ môn có tồn tại không
            var teacherDB = _db.USERs.Where(x => x.IdUser == idUser && x.IsDeleted != true &&
                            (x.IdRole == (int)RoleEnum.TEACHER || x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT))
                .FirstOrDefault();

            if (teacherDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy giáo viên này",
                };
            }

            var subjectDB = _db.SUBJECTs.Where(x => x.IdSubject == idSubject).FirstOrDefault();
            if (subjectDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy bộ môn này",
                };
            }

            // Kiểm tra đây đã là giáo viên bộ môn hay chưa
            if (teacherDB.TEACHERs.Any(x => x.TEACHER_SUBJECT.Any(s => s.IdSubject == idSubject)))
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Giáo viên này đã là giáo viên bộ môn",
                };
            }

            // Thêm vào CSDL
            subjectDB.TEACHER_SUBJECT.Add(new TEACHER_SUBJECT
            {
                IdSubject = idSubject,
                IdTeacher = teacherDB.TEACHERs.First().IdTeacher,
            });

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
        /// Xoá giáo viên bộ môn
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSubject"></param>
        /// <returns></returns>
        public BaseResponse RemoveTecherInSubject(int idUser, int idSubject)
        {
            // Kiểm tra giáo viên và bộ môn có tồn tại không
            var teacherDB = _db.USERs.Where(x => x.IdUser == idUser && x.IsDeleted != true &&
                            (x.IdRole == (int)RoleEnum.TEACHER || x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT))
                .FirstOrDefault();

            if (teacherDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy giáo viên này",
                };
            }

            // Lấy id teacher
            var idTeacher = teacherDB.TEACHERs.First().IdTeacher;

            // Kiểm tra xem teacher này có đang là gvbm không?
            var teacherSubjectDB = _db.TEACHER_SUBJECT.Where(x => x.IdTeacher == idTeacher && x.IdSubject == idSubject).FirstOrDefault();

            if (teacherSubjectDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Giáo viên này chưa là giáo viên bộ môn",
                };
            }

            // Xoá
            _db.TEACHER_SUBJECT.Remove(teacherSubjectDB);

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
        /// Thêm giáo viên ra vào danh sách trưởng bộ môn
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public BaseResponse SetHeadOfSubject(int idUser, int idSubject, int? fromYear, int? toYear)
        {
            // Kiểm tra giáo viên và bộ môn có tồn tại không
            var teacherDB = _db.USERs.Where(x => x.IdUser == idUser && x.IsDeleted != true && x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT)
                .FirstOrDefault();

            if (teacherDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy giáo viên này",
                };
            }

            var subjectDB = _db.SUBJECTs.Where(x => x.IdSubject == idSubject).FirstOrDefault();
            if (subjectDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy bộ môn này",
                };
            }

            // Kiểm tra đây đã là trưởng bộ môn hay chưa
            if (teacherDB.TEACHERs.Any(x => x.HEAD_OF_SUBJECT.Any(s => s.IdSubject == idSubject)))
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Giáo viên này đã là trưởng bộ môn",
                };
            }

            // Thêm vào CSDL
            subjectDB.HEAD_OF_SUBJECT.Add(new HEAD_OF_SUBJECT
            {
                IdSubject = idSubject,
                IdTeacher = teacherDB.TEACHERs.First().IdTeacher,
                FromYear = fromYear,
                ToYear = toYear,
            });

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
        /// Xoá giáo viên bộ môn
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSubject"></param>
        /// <returns></returns>
        public BaseResponse RemoveHeadOfSubject(int idUser, int idSubject)
        {
            // Kiểm tra giáo viên và bộ môn có tồn tại không
            var userDB = _db.USERs.Where(x => x.IdUser == idUser && x.IsDeleted != true && x.IdRole == (int)RoleEnum.HEAD_OF_SUBJECT)
                .FirstOrDefault();

            if (userDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy giáo viên này",
                };
            }

            // Lấy id teacher
            var teacherDB = userDB.TEACHERs.First();

            // Kiểm tra xem teacher này có đang là head of subject không?
            if (!teacherDB.HEAD_OF_SUBJECT.Any(x => x.IdSubject == idSubject))
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Giáo viên này chưa là trưởng bộ môn",
                };
            }

            // Xoá
            var subjectDB = teacherDB.HEAD_OF_SUBJECT.Where(x => x.IdSubject == idSubject).FirstOrDefault();
            teacherDB.HEAD_OF_SUBJECT.Remove(subjectDB);

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
        /// Lấy danh sách môn học mà người dùng được xem
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public List<SubjectViewModel> GetListSubjectByUserId(int idUser)
        {
            var userDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == idUser && x.IsDeleted != true).FirstOrDefault();
            if (userDB is null)
            {
                return null;
            }

            var teacherDB = userDB.TEACHERs.FirstOrDefault();

            // Tuỳ vào role gì mà trả về data phù hợp
            switch (userDB.IdRole)
            {
                // Role manager được quyền xem toàn bộ môn học
                case (int)RoleEnum.MANAGER:
                case (int)RoleEnum.ASSISTANT:
                    return _db.SUBJECTs.AsNoTracking()
                                        .OrderByDescending(x => x.IdSubject)
                                        .ToList()
                                        .Select(x => ConvertToViewModel(x))
                                        .ToList();
                // Role head of teacher hoặc teacher chỉ được xem các môn học mình được add vào
                case (int)RoleEnum.HEAD_OF_SUBJECT:
                    return teacherDB.HEAD_OF_SUBJECT.OrderByDescending(x => x.IdSubject)
                                        .ToList()
                                        .Select(x => ConvertToViewModel(x.SUBJECT))
                                        .ToList();

                case (int)RoleEnum.TEACHER:
                    return teacherDB.TEACHER_SUBJECT.OrderByDescending(x => x.IdSubject)
                                        .ToList()
                                        .Select(x => ConvertToViewModel(x.SUBJECT))
                                        .ToList();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Hàm lấy toàn bộ danh sách môn học
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse GetAllSubject()
        {
            var subjectDB = _db.SUBJECTs.OrderByDescending(x => x.IdSubject).ToList();
            return new BaseResponse
            {
                IsSuccess = true,
                Object = subjectDB.Select(x => ConvertToViewModel(x)).ToList(),
            };
        }

        public BaseResponse GetAllSubjectAndTeacher()
        {
            var result = new List<SubjectAndTeacherViewModel>();

            var subjectDB = _db.SUBJECTs.OrderByDescending(x => x.IdSubject).ToList();

            foreach (var subject in subjectDB)
            {
                var data = new SubjectAndTeacherViewModel
                {
                    Teacher = new List<UserViewModel>(),
                };
                data.Subject = ConvertToViewModel(subject);

                // Teacher in subject
                foreach (var teacher in subject.TEACHER_SUBJECT)
                {
                    var userVM = ConvertToViewModel(teacher.TEACHER.USER);
                    userVM.IdTeacherSubject = teacher.IdTeacherSubject;
                    data.Teacher.Add(userVM);
                }

                result.Add(data);
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = result,
            };
        }
    }
}
