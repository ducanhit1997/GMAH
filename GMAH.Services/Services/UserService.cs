using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GMAH.Services.Services
{
    public class UserService : BaseService
    {
        /// <summary>
        /// Lấy toàn bộ danh sách user theo role
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<USER> GetUsersByRole(RoleEnum[] roles)
        {
            return _db.USERs.Where(x => roles.Any(r => (int)r == x.IdRole) && x.IsDeleted != true).ToList();
        }

        public List<UserViewModel> GetUsersViewModelByRole(RoleEnum[] roles)
        {
            return GetUsersByRole(roles).Select(x => ConvertToViewModel(x)).ToList();
        }

        /// <summary>
        /// Phân trang danh sách người dùng
        /// </summary>
        /// <param name="role"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaginationResponse PaginationUserByRole(RoleEnum[] roles, DatatableParam filter)
        {
            // Lấy danh sách
            var listUser = GetUsersByRole(roles);

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listUser = listUser.Where(x => x.Fullname.Contains(value) || x.Username.Contains(value)).ToList();
            }

            // Phân trang
            var data = listUser.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách theo role
            var listUserAferConvert = new List<UserViewModel>();
            foreach (var user in data)
            {
                listUserAferConvert.Add(ConvertToViewModel(user));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listUser.Count(),
                recordsFiltered = listUserAferConvert.Count,
                data = listUserAferConvert,
            };
        }

        /// <summary>
        /// Lưu hoặc tạo mới 1 user
        /// </summary>
        /// <param name="data"></param>
        public SaveUserInfoResponse SaveUser(SaveUserInfoRequest data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new SaveUserInfoResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            // Tìm user trong db
            var userDB = _db.USERs.Where(x => x.IdUser == data.IdUser && x.IsDeleted != true).FirstOrDefault();

            if (userDB is null)
            {
                // Nếu user không tồn tại và là lưu cũ thì báo lỗi
                if (!data.IsCreateNew)
                {
                    return new SaveUserInfoResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng này",
                    };
                }
                else
                {
                    // Ngược lại tạo ra 1 user mới cho db và thêm vào db
                    userDB = new USER();
                    _db.USERs.Add(userDB);
                }
            }

            // Sửa các dữ liệu
            // Không đổi username nếu tài khoản đã được tạo trước đó
            userDB.IdRole = data.IdRole;
            userDB.Fullname = data.Fullname;
            userDB.Phone = data.Phone;
            userDB.Email = data.Email;
            userDB.Address = data.Address;
            userDB.CitizenId = data.CitizenId;

            // Kiểm tra password nếu có
            if (!string.IsNullOrEmpty(data.Password) || !string.IsNullOrEmpty(data.Repassword))
            {
                // Password hoặc nhập lại pass sai
                if (data.Password is null || data.Repassword is null || !data.Password.Equals(data.Repassword))
                {
                    return new SaveUserInfoResponse
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu và xác nhận mật khẩu không trùng khớp",
                    };
                }

                // Lưu password
                userDB.HashPassword = HashUtility.ToHashedString(data.Password);
            }

            // Kiểm tra tài khoản đã tồn tại
            if (data.IsCreateNew)
            {
                if (_db.USERs.Any(x => x.Username.Equals(data.Username, StringComparison.OrdinalIgnoreCase) && x.IsDeleted != true))
                {
                    return new SaveUserInfoResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản này đã có người dùng, vui lòng chọn tài khoản khác",
                    };
                }
                else
                {
                    userDB.Username = data.Username;
                }
            }

            // Đổi student code hoặc teacher code
            switch (data.IdRole)
            {
                case (int)RoleEnum.TEACHER:
                case (int)RoleEnum.HEAD_OF_SUBJECT:
                    var teacherDB = _db.TEACHERs.Where(x => x.IdUser == data.IdUser).FirstOrDefault();
                    if (teacherDB is null)
                    {
                        teacherDB = new TEACHER();
                        userDB.TEACHERs.Add(teacherDB);
                    }

                    // Kiểm tra teacher code tồn tại chưa
                    if (_db.TEACHERs.Any(x => x.TeacherCode.Equals(data.TeacherCode, StringComparison.OrdinalIgnoreCase) && x.IdUser != data.IdUser))
                    {
                        return new SaveUserInfoResponse
                        {
                            IsSuccess = false,
                            Message = "Mã giáo viên đã tồn tại",
                        };
                    }

                    teacherDB.TeacherCode = data.TeacherCode;
                    break;

                case (int)RoleEnum.STUDENT:
                    var studentDB = _db.STUDENTs.Where(x => x.IdUser == data.IdUser).FirstOrDefault();
                    if (studentDB is null)
                    {
                        studentDB = new STUDENT();
                        userDB.STUDENTs.Add(studentDB);
                    }

                    // Kiểm tra student code tồn tại chưa
                    if (_db.STUDENTs.Any(x => x.StudentCode.Equals(data.StudentCode, StringComparison.OrdinalIgnoreCase) && x.IdUser != data.IdUser))
                    {
                        return new SaveUserInfoResponse
                        {
                            IsSuccess = false,
                            Message = "Mã học sinh đã tồn tại",
                        };
                    }

                    studentDB.StudentCode = data.StudentCode;
                    break;
            }

            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Nếu lưu thành công thì lưu các dữ liệu đi kèm
                // Phụ huynh thì lưu thêm học sinh của phụ huynh
                if (data.IdRole == (int)RoleEnum.PARENT)
                {
                    var parentService = new ParentService();
                    parentService.SetListChild(userDB.IdUser, data.IdChilds ?? new List<int>());
                }
                else if (data.IdRole == (int)RoleEnum.STUDENT)
                {
                    var parentService = new ParentService();
                    parentService.SetListParent(userDB.IdUser, data.IdParents ?? new List<int>());
                }

                // Thành công
                return new SaveUserInfoResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new SaveUserInfoResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Lấy thông tin user bằng Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse GetUserById(int id)
        {
            var userDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == id && x.IsDeleted != true).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (userDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại",
                };
            }

            // Trả về user
            return new BaseResponse
            {
                IsSuccess = true,
                Object = ConvertToViewModel(userDB),
            };
        }

        /// <summary>
        /// Xoá người dùng bằng Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse DeleteUserById(int id)
        {
            var userDB = _db.USERs.Where(x => x.IdUser == id && x.IsDeleted != true).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (userDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại",
                };
            }

            // Xoá bằng soft delete
            userDB.IsDeleted = true;

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
        /// Random password and send mail to user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse SendMailPassword(int id)
        {
            var userDB = _db.USERs.Where(x => x.IdUser == id && x.IsDeleted != true).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (userDB is null)
            {
                return new BaseResponse("Người dùng không tồn tại");
            }

            // Kiểm tra user có mail ko
            if (string.IsNullOrEmpty(userDB.Email))
            {
                return new BaseResponse("Người dùng này chưa có email, vui lòng cập nhật email cho người dùng");
            }

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
                return new BaseResponse("Thiết lập hệ thống không đầy đủ, vui lòng liên hệ quản trị viên");
            }

            // Random password
            var randomPassword = StringUtility.RandomString(15);
            userDB.HashPassword = HashUtility.ToHashedString(randomPassword);
            _db.SaveChanges();

            // Send mail
            var emailUtility = new EmailUtility(senderName, emailUsername, emailPassword, smtp, port);
            var emailTemplate = settingService.GetSetting(SettingEnum.EMAIL_TEMPLATE_PASSWORD) ?? String.Empty;
            emailTemplate = emailTemplate.Replace("{username}", userDB.Username);
            emailTemplate = emailTemplate.Replace("{password}", randomPassword);
            emailUtility.Send(userDB.Email, "TÀI KHOẢN CỦA BẠN VỪA ĐƯỢC KHỞI TẠO", emailTemplate);

            return new BaseResponse
            {
                IsSuccess = true,
            };
        }

        /// <summary>
        /// Cập nhật thông tin
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse UpdateInfo(UpdateInfoRequest data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new SaveUserInfoResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            // Kiểm tra dữ liệu tồn tại
            var userDB = _db.USERs.Where(x => x.IdUser == data.IdUser && x.IsDeleted != true).FirstOrDefault();

            if (userDB is null)
            {
                return new BaseResponse("Người dùng này không tồn tại");
            }

            // Sửa info
            userDB.Email = data.Email;
            userDB.Phone = data.Phone;
            userDB.Address = data.Address;

            // Save lại data

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
        /// Cập nhật mật khẩu
        /// </summary>
        /// <returns></returns>
        public BaseResponse UpdatePassword(UpdatePasswordRequest data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new SaveUserInfoResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            // Kiểm tra mật khẩu khớp ko
            if (!data.Password.Equals(data.RePassword))
            {
                return new BaseResponse("Xác nhận lại mật khẩu không trùng khớp với mật khẩu mới");
            }

            // Kiểm tra dữ liệu tồn tại
            var userDB = _db.USERs.Where(x => x.IdUser == data.IdUser && x.IsDeleted != true).FirstOrDefault();

            if (userDB is null)
            {
                return new BaseResponse("Người dùng này không tồn tại");
            }

            // Check mật khẩu hiện tại
            if (!userDB.HashPassword.Equals(HashUtility.ToHashedString(data.CurrentPassword), StringComparison.OrdinalIgnoreCase))
            {
                return new BaseResponse("Mật khẩu hiện tại không đúng");
            }

            // Sửa info
            userDB.HashPassword = HashUtility.ToHashedString(data.Password);

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
    }
}
