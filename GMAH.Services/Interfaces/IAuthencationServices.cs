using GMAH.Models.Models;

namespace GMAH.Services.Interfaces
{
    public interface IAuthencationService
    {
        /// <summary>
        /// Create a token for vaild logged user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string CreateTokenByIdentity(UserIdentity user);
    }
}
