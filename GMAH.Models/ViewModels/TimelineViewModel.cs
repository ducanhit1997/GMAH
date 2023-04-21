using System;
using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class TimelineViewModel
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string DateString { get; set; }
        public List<TimelineDetailViewModel> Detail { get; set; }
    }

    public class TimelineDetailViewModel
    {
        public string SubjectName { get; set; }
        public int Period { get; set; }
        public string TeacherFullname { get; set; }
    }
}
