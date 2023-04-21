using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class HistoryScoreViewModel
    {
        public int IdSubject { get; set; }
        public string SubjectName { get; set; }
        public List<HistoryScoreLogViewModel> Logs { get; set; }
    }
}
