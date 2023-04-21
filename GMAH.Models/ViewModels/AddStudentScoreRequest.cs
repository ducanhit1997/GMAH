using GMAH.Models.Consts;

namespace GMAH.Models.ViewModels
{
    public class AddStudentScoreRequest
    {
        public int IdUser { get; set; }
        public int IdSubject { get; set; }
        public int IdSemester { get; set; }
        public int ScoreTypeId { get; set; }
        public double? Score { get; set; }
    }
}
