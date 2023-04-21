using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class UpdatePasswordRequest
    {
        public int IdUser { get; set; }

        [MinLength(5, ErrorMessage = "Mật khẩu ít nhất 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }
    }
}
