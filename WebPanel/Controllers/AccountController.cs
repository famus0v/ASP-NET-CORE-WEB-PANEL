using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebPanel.Domain.ViewModels;
using WebPanel.Domain.Entity;
using WebPanel.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebPanel.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public ActionResult Index()
        {
            if(User.Identity != null)
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Get", new { tableId = 1 });
            }
            return RedirectToAction("Index", "Home");
        }

       // [Route("{controller=Account}/{action=Get}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("Index", "Home");

            var response = await _accountService.GetAllUsers();
            if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Account viewModel)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var response = await _accountService.CreateUser(viewModel);
                if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                {
                    return RedirectToAction("Get");
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            return View(viewModel);
            
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.LoginUser(viewModel);
                if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Get");
                }
                else
                {
                    ModelState.AddModelError("", response.Description);
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string name)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.DeleteUser(name);
                //if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                //{
                //    return RedirectToAction("Get");
                //}
            }
            return RedirectToAction("Get");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
