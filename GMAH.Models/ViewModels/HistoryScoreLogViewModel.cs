using System;

namespace GMAH.Models.ViewModels
{
    public class HistoryScoreLogViewModel
    {
        public string DateUpdate { get; set; }
        public int UpdateBy { get; set; }
        public string UpdateByName { get; set; }
        public string Log { get; set; }
        public DateTime LogDate { get; set; }
    }
}
