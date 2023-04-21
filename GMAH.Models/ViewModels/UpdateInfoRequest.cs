using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class UpdateInfoRequest
    {
        public int IdUser { get; set; }

        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Vui lòng nhập đúng định dạng điện thoại")]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
