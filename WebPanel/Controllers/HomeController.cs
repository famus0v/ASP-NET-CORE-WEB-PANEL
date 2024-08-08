using Microsoft.AspNetCore.Mvc;

namespace WebPanel.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        public IActionResult Index()
        {

            if (User.Identity!=null)
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "OtherTables");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}