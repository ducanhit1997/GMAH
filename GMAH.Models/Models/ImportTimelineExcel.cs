using System.Collections.Generic;

namespace GMAH.Models.Models
{
    public class ImportTimelineExcel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<TimelineData> TimelineData { get; set; }
    }
}
