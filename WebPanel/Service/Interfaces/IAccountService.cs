using System.Security.Claims;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;
using WebPanel.Domain.ViewModels;

namespace WebPanel.Service.Interfaces
{
    public interface IAccountService
    {
        Task<IBaseResponse<IEnumerable<Account>>> GetAllUsers();
        Task<IBaseResponse<Account>> CreateUser(Account viewModel);
        Task<IBaseResponse<Account>> EditElement(Account viewModel);
        Task<IBaseResponse<Account>> GetUserById(int id);
        Task<IBaseResponse<Account>> GetUserByName(string name);
        Task<IBaseResponse<bool>> DeleteUser(int id);
        Task<IBaseResponse<bool>> DeleteUser(string name);
        Task<IBaseResponse<ClaimsIdentity>> LoginUser(LoginViewModel viewModel);


    }
}
