using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMAH.Models.ViewModels
{
    public class GetGradeRuleResponse : BaseResponse
    {
        public int IdSemester { get; set; }
        public List<int> IdClass { get; set; }
    }
}
