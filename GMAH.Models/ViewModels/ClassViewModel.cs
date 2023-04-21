using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class ClassViewModel
    {
        public int IdClass { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm học cho lớp học")]
        public int? IdYear { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên lớp học")]
        public string ClassName { get; set; }

        public string YearName { get; set; }
        public int? IdFormTeacher { get; set; }
        public string FormTeacherFullname { get; set; }
        public List<ClassSubjectTeacherViewModel> Subject { get; set; }
    }
}
