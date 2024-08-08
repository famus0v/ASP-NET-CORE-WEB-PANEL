using System.ComponentModel.DataAnnotations;

namespace WebPanel.Domain.ViewModels
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(20, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Имя должно иметь длину больше 3 символов")]
        [Display(Name = "Логин")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Подтвердить пароль")]
        [Required(ErrorMessage = "Введите пароль ещё раз")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Полное имя")]
        [Required(ErrorMessage = "Введите полное имя")]
        public string FullName { get; set; }
    }
}
