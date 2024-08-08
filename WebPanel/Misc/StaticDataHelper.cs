using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using WebPanel.DAL;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Enum;

namespace WebPanel.Misc
{
    public static class StaticDataHelper
    {
        private static IServiceProvider? _serviceProvider;

        private static IServiceScope _scope;
        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scope = _serviceProvider?.CreateScope();
            SaveColorsData();
        }

        public async static Task UpdateUserActivity(string? userName)
        {
            if (string.IsNullOrEmpty(userName)) return;


            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Account? _user = null;

            if (dbContext != null)
            {
                _user = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Name == userName);

                if (_user != null)
                {
                    _user.LastActivity = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public static int GetUserElementsCount(string? userName)
        {
            if (string.IsNullOrEmpty(userName)) return 0;


            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<TableElement>? _elements = null;

            if (dbContext != null)
            {
                _elements = dbContext.TableElements.Where(x => x.OwnerName == userName).ToList();

                if (_elements != null && _elements.Count >= 1)
                {
                    return _elements.Count;
                }
            }
            return 0;
        }

        public static string[] GetColorTimes(Colors color)
        {
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<string> dates = new List<string>();

            string lastTime = "null";

            foreach (var _colorData in dbContext.ColorData.Where(x => x.Color == color))
            {
                if (lastTime == _colorData.Date.ToString("D")) continue;
                dates.Add(_colorData.Date.ToString("D"));
                lastTime = _colorData.Date.ToString("D");
            }

            return dates.ToArray();
        }

        public static int[] GetColorCount(Colors color)
        {
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<int> counts = new List<int>();

            foreach (var _colorData in dbContext.ColorData.Where(x => x.Color == color))
            {
                counts.Add(_colorData.Count);
            }

            return counts.ToArray();
        }

        public async static Task SaveColorsData()
        {
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var redCount = dbContext.TableElements.Where(x => x.Color == Colors.Red).ToList().Count;
            var greenCount = dbContext.TableElements.Where(x => x.Color == Colors.Green).ToList().Count;
            var yellowCount = dbContext.TableElements.Where(x => x.Color == Colors.Yellow).ToList().Count;

            var red = new ColorDataInfo()
            {
                Date = DateTime.Now,
                Color = Colors.Red,
                Count = redCount
            }; 
            var green = new ColorDataInfo()
            {
                Date = DateTime.Now,
                Color = Colors.Green,
                Count = greenCount
            };
            var yellow = new ColorDataInfo()
            {
                Date = DateTime.Now,
                Color = Colors.Yellow,
                Count = yellowCount
            };

            await dbContext.ColorData.AddAsync(red);
            await dbContext.ColorData.AddAsync(green);
            await dbContext.ColorData.AddAsync(yellow);

            await dbContext.SaveChangesAsync();
        }

        public async static Task<FileModel?> GetFile(int id)
        {
            FileModel? file = null;

            
                var dbContext = _scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if(dbContext != null)
                file = await dbContext.Files.FirstOrDefaultAsync(x => x.Id == id);
            

            return file;
        }

        public static List<FileModel>? GetFiles(string json)
        {
            List<FileModel>? files = null;

            List<int> ids = JsonListConverter.GetListIntoString(json);

            
                var dbContext = _scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                files = dbContext?.Files.Where(x => ids.Contains(x.Id)).ToList();
            

            return files;
        }

        public static async Task<string>? GetUserFullName(string userIdentityName)
        {
            string? name = "unknown";
            
                var dbContext = _scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (dbContext != null)
                {
                    var _user = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Name == userIdentityName);

                    if (_user != null)
                        name = _user.FullName;
                }
            

            return name;
        }

        public static async Task<bool> AccountExist(string userIdentityName)
        {

            var dbContext = _scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext != null)
            {
                var _user = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Name == userIdentityName);

                if (_user != null)
                    return true;
            }


            return false;
        }

    }
}
