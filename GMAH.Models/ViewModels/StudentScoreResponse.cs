using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class StudentScoreResponse : BaseResponse
    {
        public List<StudentScoreDetail> ScoreComponents { get; set; }
    }

    public class StudentScoreDetail
    {
        public double? Score { get; set; }
        public string ScoreName { get; set; }
    }
}
