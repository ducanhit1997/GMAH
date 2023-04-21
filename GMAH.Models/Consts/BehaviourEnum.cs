using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.Consts
{
    /// <summary>
    /// Hạnh kiểm của học sinh
    /// </summary>
    public enum BehaviourEnum
    {
        [Display(Name = "Tốt")]
        VERYGOOD,
        [Display(Name = "Khá")]
        GOOD,
        [Display(Name = "Trung bình")]
        AVERAGE,
        [Display(Name = "Yếu")]
        BAD
    }
}
