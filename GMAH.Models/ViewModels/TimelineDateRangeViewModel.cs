using System;

namespace GMAH.Models.ViewModels
{
    public class TimelineDateRangeViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromJs => DateFrom.ToString("yyyy-MM-ddTHH:mm:ss");
        public string DateToJs => DateTo.ToString("yyyy-MM-ddTHH:mm:ss");

        public string DateFromString => DateFrom.ToString("dd/MM/yyyy");
        public string DateToString => DateTo.ToString("dd/MM/yyyy");
        public bool IsCurrentWeek { get; set; }
    }
}
