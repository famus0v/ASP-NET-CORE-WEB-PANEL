using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;

namespace WebPanel.Service.Interfaces
{
    public interface IBaseTableService
    {
        Task<IBaseResponse<string>> GetTableName(int tableId);
        Task<IBaseResponse<List<BaseTable>>> GetAllTables();
        Task<IBaseResponse<BaseTable>> CreateTable(BaseTable viewModel);
        Task<IBaseResponse<BaseTable>> DeleteTable(int tableId);
    }
}
