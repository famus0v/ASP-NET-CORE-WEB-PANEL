using Microsoft.AspNetCore.Mvc;
using WebPanel.Domain.Entity;
using WebPanel.Service.Interfaces;
using WebPanel.Domain.ViewModels;
using WebPanel.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using WebPanel.Misc;
using X.PagedList;

namespace WebPanel.Controllers
{
    public class TableController : Controller
    {

        private readonly ITableElementService _elementService;
        private readonly IBaseTableService _basetableService;
        private readonly IFileModelService _filemodelService;
        public TableController(ITableElementService elementService, IBaseTableService basetableService, IFileModelService filemodelService)
        {
            _elementService = elementService;
            _basetableService = basetableService;
            _filemodelService = filemodelService;
        }

        [Authorize]
        public ActionResult Index()
        {
              return RedirectToAction("Get", new { tableId = 1 });
        }

        [Route("{controller=Home}/{tableId:int}/{elementId}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Element(int tableId, int elementId)
        {
            var baseTable = await _basetableService.GetTableName(tableId);
            if(baseTable.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                ViewData["Title"] = baseTable.Data;

            ViewData["TableId"] = tableId;

            var response = await _elementService.GetElementById(elementId);
            if(response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        



    [Route("{controller=Home}/{tableId:int}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(FindViewModel viewModel, int tableId, string acceptID, int pageNumber = 1, int pageSize = 9)
        {
            var accountExist = await StaticDataHelper.AccountExist(User.Identity.Name);
            if (!accountExist)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            var baseTable = await _basetableService.GetTableName(tableId);
            if(baseTable.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                ViewData["Title"] = baseTable.Data;
            ViewData["TableId"] = tableId;
            ViewData["AcceptID"] = acceptID;

            viewModel.TableId = tableId;

            // Передаем модель представления с данными и информацией о пагинации
            viewModel.PageNumber = pageNumber;
            viewModel.PageSize = pageSize;

            var response = await _elementService.GetElements(viewModel);
            if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
            {
                // Применяем пагинацию к результатам запроса
                viewModel.Output = response.Data.ToPagedList(pageNumber, pageSize);
                viewModel.TotalItemCount = viewModel.Output.TotalItemCount;
                return View(viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }

        [Route("{controller=Home}/{tableId:int}/{action=Index}")]
        [Authorize]
        public ActionResult Create(int tableId)
        {
            ViewData["TableId"] = tableId;
            return View();
        }

        [Route("{controller=Home}/{tableId:int}/{action=Index}")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int tableId, TableElement viewModel)
        {
            ViewData["TableId"] = tableId;

            ModelState.Remove("Id");
            ModelState.Remove("DateOfChanges");
            ModelState.Remove("OwnerFullName");
            ModelState.Remove("OwnerName");
            ModelState.Remove("ElementPath");

            if (ModelState.IsValid)
            {
                var response = await _elementService.AddElement(viewModel,tableId,User.Identity?.Name ?? "admin");
                if (response.StatusCode != TryWebSite.Domain.Enum.StatusCode.OK)
                {
                    ModelState.AddModelError("", response.Description);
                    return View(response.Data);
                }
            }
            else
            {
                return View(viewModel);
            }
            return RedirectToAction("Get", new { tableId });
        }

        [Route("{controller=Home}/{tableId:int}/{action=Index}")]
        [Authorize]
        public async Task<IActionResult> Edit(int elementId, int tableId)
        {
            ViewData["TableId"] = tableId;

            var response = await _elementService.GetElementById(elementId);
            if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            return RedirectToAction("Error");
        }

        [Route("{controller=Home}/{tableId:int}/{action=Index}")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int tableId, TableElement viewModel)
        {
            ViewData["TableId"] = tableId;

            ModelState.Remove("Id");
            ModelState.Remove("DateOfChanges");
            ModelState.Remove("OwnerFullName");
            ModelState.Remove("OwnerName");

            if (ModelState.IsValid)
            {
                var response = await _elementService.EditElement(viewModel);
                if (response.StatusCode != TryWebSite.Domain.Enum.StatusCode.OK)
                {
                    ModelState.AddModelError("", response.Description);
                    return View(response.Data);
                }
            }
            else
            {
                return View(viewModel);
            }

            return RedirectToAction("Get", new { tableId });
        }

        [Route("{controller=Home}/{tableId:int}/{action=Index}/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int tableId, int id)
        {
            var response = await _elementService.DeleteElement(id);
            if (response.StatusCode == TryWebSite.Domain.Enum.StatusCode.OK)
                return RedirectToAction("Get", new { tableId });
            return RedirectToAction("Get",new { tableId});
        }


        [Route("{controller=Home}/{tableId:int}/{elementId:int}/{action=Index}/{fileType:int}")]
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(3221225472)]
        public async Task<IActionResult> AddFile(int tableId,int elementId, int fileType, IFormFile file)
        {
            //var file = Request.Form.Files[0];
            //if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
            //{
            //    return BadRequest("No file is uploaded.");
            //}
            
            var response = await _filemodelService.AddFile(elementId,(FileType)fileType, file);


            if (response.StatusCode != TryWebSite.Domain.Enum.StatusCode.OK)
            {
                return Json(new { isError = "true" });
            }
            return Json(new { redirectUrl = "/Table/" + tableId + "/" + elementId + $"#section-{fileType}" });
        }

        [Route("{controller=Home}/{tableId:int}/{elementId:int}/{fileId:int}/{action=Index}")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(int tableId, int elementId, int fileId)
        {

            var response = await _filemodelService.DeleteFile(elementId, fileId);


            if (response.StatusCode != TryWebSite.Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("Element", new {tableId,elementId});
            }
            return RedirectToAction("Element", new { tableId, elementId });
        }

        [Authorize]
        public IActionResult DownloadFile(string filePath,string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("Путь к файлу не указан.");
            }

            filePath = System.Net.WebUtility.UrlDecode(filePath);
            fileName = System.Net.WebUtility.UrlDecode(fileName);

            string absolutePath = Path.GetFullPath(filePath);

            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound("Файл не найден.");
            }

            //_contentTypeProvider.TryGetContentType(fileName, out var fileMime);

            return PhysicalFile(absolutePath, "application/octet-stream", fileName);
        }


    }
}
