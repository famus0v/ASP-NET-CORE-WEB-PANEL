using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO.Compression;
using WebPanel.Misc;
using WebPanel.Service.Interfaces;

namespace WebPanel.Controllers
{
    public class ExportController : Controller
    {

        private readonly ITableElementService _elementService;
        private readonly IBaseTableService _basetableService;
        public ExportController(ITableElementService elementService, IBaseTableService basetableService)
        {
            _elementService = elementService;
            _basetableService = basetableService;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [Route("{controller=Home}/{action=Index}/{elementId:int}")]
        [Authorize]
        public async Task<IActionResult> Element(int elementId)
        {
            var data = await _elementService.GetElementById(elementId);

            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            var dataType = data.Data.GetType();

            var properties = dataType.GetProperties();

            for (int columnIndex = 0; columnIndex < properties.Length; columnIndex++)
            {
                var property = properties[columnIndex];
                var columnName = property.Name;
                worksheet.Cells[1, columnIndex + 1].Value = columnName;
            }

            for (int columnIndex = 0; columnIndex < properties.Length; columnIndex++)
            {
                var property = properties[columnIndex];
                var cellValue = property.GetValue(data.Data);
                worksheet.Cells[2, columnIndex + 1].Value = cellValue;
            }

            var dateRange = worksheet.Cells[worksheet.Dimension.Start.Row, 17, worksheet.Dimension.End.Row, 19];

            // Установка формата даты и времени для диапазона ячеек
            dateRange.Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

            var fileBytes = await package.GetAsByteArrayAsync();

            string fileName = data.Data.FullName.Replace(" ", "_") + ".xlsx";

            string tempFilePath = Path.GetTempFileName();
            await System.IO.File.WriteAllBytesAsync(tempFilePath, fileBytes);

            string tempZipFilePath = Path.GetTempFileName();

            using (var zipArchive = new ZipArchive(new FileStream(tempZipFilePath, FileMode.Create), ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(tempFilePath, fileName);

                if (data.Data.ElementPath != null)
                {
                    var files = Directory.GetFiles(data.Data.ElementPath);
                    foreach (var file in files)
                    {
                        //string relativePath = Path.GetRelativePath(data.Data.ElementPath, file);
                        string relativePath = Path.Combine("Files", Path.GetFileName(file)); // Путь к файлу внутри папки
                        zipArchive.CreateEntryFromFile(file, relativePath);
                    }
                }
            }

            var zipBytes = await System.IO.File.ReadAllBytesAsync(tempZipFilePath);

            string zipContentType = "application/zip";
            string zipFileName = data.Data.FullName.Replace(" ", "_") + ".zip";

            System.IO.File.Delete(tempFilePath);
            System.IO.File.Delete(tempZipFilePath);

            return File(zipBytes, zipContentType, zipFileName);
        }

        [Route("{controller=Home}/{action=Index}/{tableId:int}")]
        [Authorize]
        public async Task<IActionResult> Table(int tableId)
        {

            var data = await _elementService.GetElementsByTableId(tableId);
            var tableName = await _basetableService.GetTableName(tableId);

            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add(tableName.Data);

            worksheet.Cells.LoadFromCollection(data.Data, true);

            var dateRange = worksheet.Cells[worksheet.Dimension.Start.Row, 17, worksheet.Dimension.End.Row, 19];

            // Установка формата даты и времени для диапазона ячеек
            dateRange.Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";


            var fileBytes = await package.GetAsByteArrayAsync();

            string fileName = tableName.Data.Replace(" ", "_") + ".xlsx";

            string tablePath = PanelFilesManager.MAIN_PATH + "/" + tableId.ToString();

            string tempFilePath = Path.GetTempFileName();
            await System.IO.File.WriteAllBytesAsync(tempFilePath, fileBytes);

            string tempZipFilePath = Path.GetTempFileName();

            using (var zipArchive = new ZipArchive(new FileStream(tempZipFilePath, FileMode.Create), ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(tempFilePath, fileName);

                if (Directory.Exists(tempFilePath))
                {
                    var files = Directory.GetFiles(tablePath, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        string archiveFilesFolder = "Files";
                        string? directoryPath = Path.GetDirectoryName(Path.GetFullPath(file));
                        string? folderName = Path.GetFileName(directoryPath);

                        //string relativePath = Path.GetRelativePath(data.Data.ElementPath, file);
                        if (folderName != null) archiveFilesFolder += $"/{folderName}";

                        string relativePath = Path.Combine(archiveFilesFolder, Path.GetFileName(file)); // Путь к файлу внутри папки
                        zipArchive.CreateEntryFromFile(file, relativePath);
                    }
                }
            }

            var zipBytes = await System.IO.File.ReadAllBytesAsync(tempZipFilePath);

            string zipContentType = "application/zip";
            string zipFileName = tableName.Data.Replace(" ", "_") + ".zip";

            System.IO.File.Delete(tempFilePath);
            System.IO.File.Delete(tempZipFilePath);

            return File(zipBytes, zipContentType, zipFileName);
        }
    }
}
