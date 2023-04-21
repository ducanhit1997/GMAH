using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class ListAttendanceViewModel
    {
        public List<string> Headers { get; set; }
        public List<StudentViewModel> Students { get; set; }
        public List<StudentAttendanceViewModel> StudentData { get; set; }
    }
}
