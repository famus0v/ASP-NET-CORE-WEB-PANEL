using WebPanel.Domain.Entity;
using WebPanel.Domain.Enum;
using WebPanel.Domain.Response;

namespace WebPanel.Service.Interfaces
{
    public interface IFileModelService
    {
        Task<IBaseResponse<FileModel>> GetFile(int id);
        Task<IBaseResponse<FileModel>> DeleteFile(int elementId,int id);
        Task<IBaseResponse<IEnumerable<FileModel>>> GetFiles(string json);
        Task<IBaseResponse<FileModel>> AddFile(int id,FileType fileType, IFormFile fileModel);

    }
}
