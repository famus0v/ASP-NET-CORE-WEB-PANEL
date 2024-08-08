using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebPanel.Domain.Enum;

namespace WebPanel.Domain.Entity
{
    public class TableElement
    {
        [Key] public int Id { get; set; }
        public int TableId { get; set; }


        [Display(Name = "Ф. И. О.")] public string FullName { get; set; }
        [Display(Name = "Город")] public string? City { get; set; }
        [Display(Name = "Ссылка")] public string? Link { get; set; }
        [Display(Name = "Примечание")] public string? Note { get; set; }
        [Display(Name = "Интересы")] public string? Interests { get; set; }
        //[Display(Name = "Номер телефона")] public string? PhoneNumber { get; set; }
        [Display(Name = "Негативные эмоц. сост.")] public string? NegativeEmotStates { get; set; }
        [Display(Name = "Пол")] public Genders Gender { get; set; }
        [Display(Name = "Цвет")] public Colors Color { get; set; }

        [Display(Name = "Связи ID")] public string? ConnectionsId { get; set; }
        [Display(Name = "Портрет ID")] public string? PictureId { get; set; }
        [Display(Name = "Признаки противоправного контента ID")] public string? IllegalContentId { get; set; }
        [Display(Name = "Выгрузка ID")] public string? LandingId { get; set; }

        [Display(Name = "Информация передана")] public bool InformationTransferred { get; set; }


        [Display(Name = "Дата рождения")] public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Дата обнаружения")] public DateTime? DateOfDetection { get; set; }
        [Display(Name = "Дата изменения")] public DateTime? DateOfChanges { get; set; }

        public string? ElementPath { get; set; }

        public string OwnerFullName { get; set; }
        public string OwnerName { get; set; }
    }
}
