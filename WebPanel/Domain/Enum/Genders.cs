using System.ComponentModel.DataAnnotations;

namespace WebPanel.Domain.Enum
{
    public enum Genders
    {
        [Display(Name = "Не выбран")]
        NoGender = 0,
        [Display(Name = "Мужской")]
        Man = 1,
        [Display(Name = "Женский")]
        Woman = 2,
        [Display(Name = "Attack helicopter Boeing AH-64 Apache")]
        Apache = 228,
    }
}
