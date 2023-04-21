using GMAH.Models.Consts;

namespace GMAH.Models.ViewModels
{
    public class ScoreTypeViewModel
    {
        public int IdScoreType { get; set; }
        public int IdClass { get; set; }
        public int IdSubject { get; set; }
        public string ScoreName { get; set; }
        public byte ScoreWeight { get; set; }
    }
}
