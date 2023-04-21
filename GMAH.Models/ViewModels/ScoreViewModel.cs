using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class ScoreViewModel
    {
        public int IdUser { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string StudentRankName { get; set; }
        public double? StudentAvgScore { get; set; }
        public List<ScoreSubjectViewModel> Subjects { get; set; }
    }
}
