using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class ScoreSubjectViewModel
    {
        public string SubjectName { get; set; }
        public string YearName { get; set; }
        public string StudentName { get; set; }

        public int IdSubject { get; set; }
        public int? IdSemester { get; set; }
        public int IdYear { get; set; }
        public List<ScoreDetailViewModel> Details { get; set; }
    }
}
