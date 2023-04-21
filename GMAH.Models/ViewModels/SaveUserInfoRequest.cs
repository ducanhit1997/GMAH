using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class SaveUserInfoRequest
    {
        public bool IsCreateNew => IdUser <= 0;
        public int IdUser { get; set; }
        public int IdRole { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản đăng nhập")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Repassword { get; set; }

        [Phone(ErrorMessage = "Vui lòng nhập đúng định dạng điện thoại")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string CitizenId { get; set; }
        public string StudentCode { get; set; }
        public string TeacherCode { get; set; }
        public List<int> IdChilds { get; set; }
        public List<int> IdParents { get; set; }
    }
}
