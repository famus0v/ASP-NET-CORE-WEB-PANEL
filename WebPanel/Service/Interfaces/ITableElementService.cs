using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;
using WebPanel.Domain.ViewModels;

namespace WebPanel.Service.Interfaces
{
    public interface ITableElementService
    {
        Task<IBaseResponse<IEnumerable<TableElement>>> GetAllElements();

        Task<IBaseResponse<IEnumerable<TableElement>>> GetElements(FindViewModel viewModel);

        Task<IBaseResponse<TableElement>> GetElementByName(string name);

        Task<IBaseResponse<TableElement>> GetElementById(int id);

        Task<IBaseResponse<IEnumerable<TableElement>>> GetElementsByTableId(int tableId);

        Task<IBaseResponse<TableElement>> AddElement(TableElement resourceViewModel, int tableId,string userIdentityName);

        Task<IBaseResponse<bool>> DeleteElement(int id);

        Task<IBaseResponse<TableElement>> EditElement(TableElement resourceViewModel);

    }
}
