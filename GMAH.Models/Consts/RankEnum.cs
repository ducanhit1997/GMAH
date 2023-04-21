using System.ComponentModel.DataAnnotations;

namespace GMAH.Models.Consts
{
    public enum RankEnum
    {
        [Display(Name = "Xuất sắc")]
        EXCELLENT = 1,
        [Display(Name = "Giỏi")]
        VERY_GOOD,
        [Display(Name = "Khá")]
        GOOD,
        [Display(Name = "Trung bình")]
        AVERAGE,
        [Display(Name = "Yếu")]
        POOR
    }
}
