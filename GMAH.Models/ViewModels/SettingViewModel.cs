using GMAH.Models.Consts;

namespace GMAH.Models.ViewModels
{
    public class SettingViewModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public InputTypeEnum Type { get; set; }
    }
}
