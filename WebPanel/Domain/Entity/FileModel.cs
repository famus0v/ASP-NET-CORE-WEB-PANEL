using System.ComponentModel.DataAnnotations.Schema;
using WebPanel.Domain.Enum;

namespace WebPanel.Domain.Entity
{
    public class FileModel
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        [NotMapped] public IFormFile? FormFile { get; set; }
        public string? FilePath { get; set; }
        public FileType? FileType { get; set; }
    }
}
