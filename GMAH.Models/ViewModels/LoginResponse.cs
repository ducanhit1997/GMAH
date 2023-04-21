using GMAH.Models.Models;

namespace GMAH.Models.ViewModels
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }
        public UserIdentity User { get; set; }
    }
}
