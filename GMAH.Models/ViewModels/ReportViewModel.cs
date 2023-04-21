using GMAH.Models.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class ReportViewModel
    {
        public string Issue { get; set; }
        public int IdReport { get; set; }
        public ReportTypeEnum ReportType { get; set; }
        public string ReportTypeName => ReportType == ReportTypeEnum.SCORE ? "Điểm số" : "Điểm danh";

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string ReportTitle { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string ReportContent { get; set; }
        public ReportStatusEnum ReportStatus { get; set; }
        public string ReportStatusName { get; set; }

        public int IdUserSubmitReport { get; set; }
        public int SubmitForIdUser { get; set; }
        public string FullnameStudent { get; set; }

        public string FullnameSubmitReport { get; set; }
        public string EditInfo { get; set; }

        public DateTime? SubmitDate { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public string SubmitDateString => SubmitDate?.ToString("dd/MM/yyyy lúc HH:mm") ?? string.Empty;
        public string LastUpdateDateString => LastUpdateDate?.ToString("dd/MM/yyyy lúc HH:mm") ?? string.Empty;


        public List<ReviewViewModel> History { get; set; }
        public string EditField { get; set; }
        public string EditValue { get; set; }
        public List<string> Files { get; set; }
    }
}
