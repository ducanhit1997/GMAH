using System.Collections.Generic;

namespace GMAH.Models.Models
{
    public class UpdateAvgSubjectForStudentResult
    {
        public int? IdBehaviour { get; set; }
        public int? IdSemester { get; set; }
        public int IdYear { get; set; }

        public bool CanUpdateSemesterScore { get; set; }
        public List<AvgSubjectForStudentData> Scores { get; set; }
    }
}
