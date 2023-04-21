using GMAH.Models.Models;
using System;

namespace GMAH.Models.ViewModels
{
    public class StudentAttendanceViewModel
    {
        public DateTime AttendanceDate { get; set; }
        public int IdStudent { get; set; }
        public string Fullname { get; set; }
        public string StudentCode { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public int AssistantID { get; set; }
        public string AssistantName { get; set; }
        public string DateString { get; set; }
    }
}
