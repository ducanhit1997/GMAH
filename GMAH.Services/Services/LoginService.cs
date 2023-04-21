using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Interfaces;
using GMAH.Services.Utilities;
using System;
using System.Linq;

namespace GMAH.Services.Services
{
    public class LoginService : BaseService, ILoginService
    {
        /// <summary>
        /// Đăng nhập và tạo token cho người dùng
        /// </summary>
        /// <param name="username"></param>
        /// <param name="plainPassword"></param>
        /// <param name="authencationService"></param>
        /// <returns></returns>
        public LoginResponse LoginGetToken(string username, string plainPassword, IAuthencationService authencationService)
        {
            try
            {
                var identity = LoginWithPassword(username, plainPassword);
                return new LoginResponse
                {
                    IsSuccess = true,
                    User = identity,
                    Token = authencationService.CreateTokenByIdentity(identity),
                };
            }
            catch (Exception ex)
            {
                // Return error
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        /// <summary>
        /// Đăng nhập và trả về identity user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UserIdentity LoginWithPassword(string username, string plainPassword)
        {
            // Tìm user dựa vào username và hashed password
            // Không phân biệt chữ hoa chữ thường
            var userDB = _db.USERs.Where(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && x.IsDeleted != true).FirstOrDefault();

            // Nếu user không tồn tại thì throw exception với error message
            if (userDB is null)
            {
                throw new Exception("Người dùng này không tồn tại");
            }    

            // Kiểm tra hashed password
            var hashedPassword = HashUtility.ToHashedString(plainPassword);
            if (!userDB.HashPassword.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Mật khẩu không đúng");
            }

            // Trả về user identity
            return new UserIdentity
            {
                IdRole = userDB.IdRole,
                IdUser = userDB.IdUser,
                Fullname = userDB.Fullname,
                Username = userDB.Username,
                Permission = userDB.ROLE?.PERMISSIONs?.Select(x => x.ActionName).ToArray() ?? null
            };
        }
    }
}
