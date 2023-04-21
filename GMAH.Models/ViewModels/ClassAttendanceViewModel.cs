using System;
using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class ClassAttendanceViewModel
    {
        public int AssistantID { get; set; }
        public string AssistantName { get; set; }
        public int IdClass { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<StudentAttendanceViewModel> Students { get; set; }
    }
}
