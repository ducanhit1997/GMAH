using GMAH.Models.Models;
using GMAH.Models.ViewModels;

namespace GMAH.Services.Interfaces
{
    public interface ILoginService
    {
        /// <summary>
        /// Check a exist user by using username and plain password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        UserIdentity LoginWithPassword(string username, string plainPassword);
        LoginResponse LoginGetToken(string username, string plainPassword, IAuthencationService authencationService);
    }
}
