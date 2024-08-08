using System.ComponentModel.DataAnnotations;

namespace WebPanel.Domain.Enum
{
    public enum Colors
    {
        [Display(Name = "Красный")]
        Red = 0,
        [Display(Name = "Зеленый")]
        Green = 1,
        [Display(Name = "Жёлтый")]
        Yellow = 2,
        [Display(Name = "Нет цвета")]
        NoColor = 404,
    }
}
