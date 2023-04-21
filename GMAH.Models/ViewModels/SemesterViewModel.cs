using System;
using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class SemesterViewModel
    {
        public string TitleName => (IsYear ? "Năm học " + SemesterName : (SemesterName + (string.IsNullOrEmpty(SemesterYear) ? string.Empty : $" ({SemesterYear})")));
        public int IdSemester { get; set; }
        public bool IsYear { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên học kỳ")]
        public string SemesterName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập năm học")]
        public string SemesterYear { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập trọng số điểm trung bình")]
        [Range(0, 100, ErrorMessage = "Trọng số điểm trung bình từ 0 đến 100")]
        public int? ScoreWeight { get; set; }

        public bool IsCurrentSemester { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        // Convert to string
        public string DateStartText => DateStart?.ToString("dd/MM/yyyy") ?? string.Empty;
        public string DateEndText => DateEnd?.ToString("dd/MM/yyyy") ?? string.Empty;
    }
}
