using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class SubjectViewModel
    {
        public int IdSubject { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên môn học")]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã môn học")]
        public string SubjectCode { get; set; }
    }
}
