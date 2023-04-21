using System;

namespace GMAH.Models.Models
{
    public class ReportEditFieldData
    {
        public class EditScore
        {
            public int IdScoreType { get; set; }
            public int IdSubject { get; set; }
            public int IdClass { get; set; }
            public int IdSemester { get; set; }
        }

        public class EditAttendance
        {
            public int IdSemester { get; set; }
            public int IdClass { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
