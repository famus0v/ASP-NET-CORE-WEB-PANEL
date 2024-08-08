using System.ComponentModel.DataAnnotations;
using WebPanel.Domain.Enum;
using X.PagedList;

namespace WebPanel.Domain.ViewModels
{
    public class FindViewModel
    {
        [Display(Name = "Номер таблицы")] public int? TableId { get; set; }
        [Display(Name = "Уникальный номер элемента")] public int? Id { get; set; }
        [Display(Name = "Ф. И. О.")] public string? Name { get; set; }
        [Display(Name = "Город")] public string? City { get; set; }
        [Display(Name = "Номер телефона")] public string? PhoneNumber { get; set; }
        [Display(Name = "Негативные эмоц. сост.")] public string? NegativeEmotStates { get; set; }
        [Display(Name = "Интересы")] public string? Interests { get; set; }
        [Display(Name = "Пол")] public Genders? Gender { get; set; }
        [Display(Name = "Цвет")] public Colors? Color { get; set; }
        public bool InformationTransferred { get; set; }
        [Display(Name = "Дата рождения")] public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Дата обнаружения")] public DateTime? DateOfDetection { get; set; }
        [Display(Name = "Дата изменения")] public DateTime? DateOfChanges { get; set; }
        [Display(Name = "Создатель элемента")] public string? Owner { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }

        public IPagedList<Entity.TableElement>? Output { get; set; }

    }
}
