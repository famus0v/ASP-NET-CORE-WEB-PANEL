using System.ComponentModel.DataAnnotations;

namespace WebPanel.Domain.Entity
{
    public class BaseTable
    {
        public int Id { get; set; }
        [Display(Name = "Название таблицы")]
        public string TableDisplayName { get; set; }

    }
}
