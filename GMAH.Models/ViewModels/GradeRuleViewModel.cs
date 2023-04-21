using GMAH.Models.Consts;
using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class GradeRuleViewModel
    {
        public int IdRule { get; set; }
        public List<int> IdClass { get; set; }
        public string ClassName { get; set; }
        public int IdRuleList { get; set; }
        public RankEnum IdRank { get; set; }
        public int IdSemester { get; set; }
        public string GradeName { get; set; }
        public double MinAvgScore { get; set; }
        public int? IdBehaviour { get; set; }
        public string BehaviourName { get; set; }

        public List<RuleDetailViewModel> Details { get; set; }

        public override bool Equals(object obj)
        {
            try
            {
                return ((GradeRuleViewModel)obj).IdRule == IdRule;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return IdRule;
        }
    }
}
