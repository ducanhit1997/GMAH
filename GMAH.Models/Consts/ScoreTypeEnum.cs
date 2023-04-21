using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.Consts
{
    public enum ScoreTypeEnum
    {
        [Display(Name = "User defined")]
        UserDefine,
        [Display(Name = "ĐTB")]
        Avg = 101,
        [Display(Name = "GK")]
        Midterm = 102,
        [Display(Name = "CK")]
        Final = 103,
    }
}
