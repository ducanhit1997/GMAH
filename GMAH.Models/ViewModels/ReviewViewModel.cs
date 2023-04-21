using GMAH.Models.Consts;
using System;

namespace GMAH.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int IdReportHistory { get; set; }
        public int IdReport { get; set; }
        public DateTime? HistoryDate { get; set; }
        public string HistoryDateString => HistoryDate?.ToString("dd/MM/yyyy lúc HH:mm") ?? string.Empty;

        public int IdUserUpdate { get; set; }
        public string FullnameUserUpdate { get; set; }
        public string ReportStatusName { get; set; }


        public ReportStatusEnum ReportStatus { get; set; }
        public string Comment { get; set; }
    }
}
