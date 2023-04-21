using System;
using System.Collections.Generic;

namespace GMAH.Models.Models
{
    public class TimelineData
    {
        public string SubjectCode { get; set; }
        public int IdSubject { get; set; }
        public List<int> Periods { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
