using Automarket.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TryWebSite.Domain.Enum;
using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;
using WebPanel.Domain.ViewModels;
using WebPanel.Service.Interfaces;

namespace WebPanel.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<Account> _accountRepository;

        public AccountService(IBaseRepository<Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IBaseResponse<Account>> CreateUser(Account viewModel)
        {
            try
            {

                var User = new Account()
                {
                    Name = viewModel.Name,
                    FullName = viewModel.FullName,
                    Password = HashPasswordHelper.HashPassowrd(viewModel.Password),
                    Role = viewModel.Role
                };

                await _accountRepository.Add(User);

                return new BaseResponse<Account>()
                {
                    StatusCode = StatusCode.OK,
                    Data = User
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Account>()
                {
                    Description = $"[CreateUser] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteUser(int id)
        {
            var baseResponse = new BaseResponse<bool>();
            try
            {
                var element = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

                if (element == null)
                {
                    baseResponse.Description = "Resource not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                await _accountRepository.Delete(element);
                baseResponse.Data = true;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[DeleteUser] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteUser(string name)
        {
            var baseResponse = new BaseResponse<bool>();
            try
            {
                var element = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Name == name);

                if (element == null)
                {
                    baseResponse.Description = "Resource not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                await _accountRepository.Delete(element);
                baseResponse.Data = true;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[DeleteUser] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Account>> EditElement(Account viewModel)
        {
            try
            {
                var user = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Id == viewModel.Id);
                if (user == null)
                {
                    return new BaseResponse<Account>()
                    {
                        Description = "Element not found",
                        StatusCode = StatusCode.ElementNotFound
                    };
                }

                user.Name = viewModel.Name;
                user.FullName = viewModel.FullName;
                user.Password = HashPasswordHelper.HashPassowrd(viewModel.Password);

                await _accountRepository.Update(user);


                return new BaseResponse<Account>()
                {
                    Data = user,
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Account>()
                {
                    Description = $"[EditAccount] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<Account>>> GetAllUsers()
        {
            var baseResponse = new BaseResponse<IEnumerable<Account>>();
            try
            {
                var resource = await _accountRepository.GetAll().ToListAsync();
                if (resource == null)
                {
                    baseResponse.Description = "Account not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Account>>()
                {
                    Description = $"[GetAllUsers] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Account>> GetUserById(int id)
        {
            var baseResponse = new BaseResponse<Account>();
            try
            {
                var resource = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (resource == null)
                {
                    baseResponse.Description = "Account not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<Account>()
                {
                    Description = $"[GetUserById] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Account>> GetUserByName(string name)
        {
            var baseResponse = new BaseResponse<Account>();
            try
            {
                var resource = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Name == name);
                if (resource == null)
                {
                    baseResponse.Description = "Account not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<Account>()
                {
                    Description = $"[GetUserById] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<ClaimsIdentity>> LoginUser(LoginViewModel viewModel)
        {
            var baseResponse = new BaseResponse<ClaimsIdentity>();
            try
            {
                var user = await _accountRepository.GetAll().FirstOrDefaultAsync(x => x.Name == viewModel.Name);
                if (user == null)
                {
                    baseResponse.Description = "Account not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                if (user.Password != HashPasswordHelper.HashPassowrd(viewModel.Password))
                {
                    baseResponse.Description = "Password not correct";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                var result = Authenticate(user);

                baseResponse.Data = result;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = $"[GetUserById] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
                //ModelState.AddModelError("", ex.Message);
            }
        }

        private static ClaimsIdentity Authenticate(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
