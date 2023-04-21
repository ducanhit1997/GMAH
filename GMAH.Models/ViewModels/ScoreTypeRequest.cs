using GMAH.Models.Consts;
using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.ViewModels
{
    public class ScoreTypeRequest
    {
        public int IdScoreType { get; set; }
        public int IdClass { get; set; }

        public int IdSubject { get; set; }

        [Required(ErrorMessage = "Vui lòng điền tên thành phần điểm")]
        public string ScoreName { get; set; }

        public ScoreTypeEnum ScoreType { get; set; }
        public byte ScoreWeight { get; set; }
    }
}
