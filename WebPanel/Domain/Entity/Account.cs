using System.ComponentModel.DataAnnotations;
using WebPanel.Domain.Enum;

namespace WebPanel.Domain.Entity
{
    public class Account
    {
        public int Id { get; set; }
        [Display(Name = "Логин")] public string Name { get; set; }
        [Display(Name = "Пароль")] public string Password { get; set; }
        [Display(Name = "Полное имя")] public string FullName { get; set; }
        [Display(Name = "Роль")] public Role Role { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
