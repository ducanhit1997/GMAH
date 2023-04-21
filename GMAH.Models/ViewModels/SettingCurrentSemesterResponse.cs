using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class SettingCurrentSemesterResponse
    {
        public int? SelectedId { get; set; }
        public int? SelectedIdYear { get; set; }

        public string CurrentSemesterName { get; set; }
        public List<SemesterViewModel> Data { get; set; }
    }
}
