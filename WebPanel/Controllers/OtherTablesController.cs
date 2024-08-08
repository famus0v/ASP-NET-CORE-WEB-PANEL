using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPanel.Domain.Entity;
using WebPanel.Misc;
using WebPanel.Service.Interfaces;

namespace WebPanel.Controllers
{
    public class OtherTablesController : Controller
    {
        private readonly IBaseTableService _basetableService;

        public OtherTablesController(IBaseTableService basetableService)
        {
            _basetableService = basetableService;
        }

        [Authorize]
        public async  Task<IActionResult> Index()
        {
            var accountExist = await StaticDataHelper.AccountExist(User.Identity.Name);
            if (!accountExist)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            var response = await _basetableService.GetAllTables();

            return View(response.Data);
        }

        [Authorize]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin")) return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(BaseTable viewModel)
        {
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                var response = await _basetableService.CreateTable(viewModel);
                if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", response.Description);
                    return View(response.Data);
                }

            }
            else
            {
                return View(viewModel);
            }
        }

        [Authorize]
        [Route("{controller=Home}/{action=Index}/{tableId:int}")]
        public async Task<IActionResult> Delete(int tableId)
        {
            var response = await _basetableService.DeleteTable(tableId);
            if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                return RedirectToAction("Index");
            return RedirectToAction("Index");
        }
    }
}
