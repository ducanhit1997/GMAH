using GMAH.Models.Consts;
using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class ScoreDetailViewModel
    {
        public int? IdScore { get; set; }
        public int IdScoreType { get; set; }
        public int ScoreWeight { get; set; }
        public string ScoreName { get; set; }
        public double? Score { get; set; }
        public string Text { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsOption { get; set; }
        public List<OptionViewModel> ListOption { get; set; }
        public string SelectedValueOption { get; set; }
        public string Note { get; set; }
    }
}
