using WebPanel.Domain.Enum;

namespace WebPanel.Domain.Entity
{
    public class ColorDataInfo
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Colors Color { get; set; }
        public int Count { get; set; }
    }
}
