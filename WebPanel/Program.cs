using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using WebPanel;
using WebPanel.DAL;
using WebPanel.DAL.Interfaces;
using WebPanel.DAL.Repositories;
using WebPanel.Domain.Entity;
using WebPanel.Misc;
using WebPanel.Misc.TaskTimer;
using WebPanel.Service.Implementations;
using WebPanel.Service.Interfaces;

Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel((hostingContext, options) =>
            {
                options.Limits.MaxRequestBodySize = 2147483648; // 2 гигабайта
            });
        });

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/Login");
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 3221225472; // Устанавливаем максимальный размер тела запроса в байтах (например, 100 МБ)

});

builder.Services.AddSingleton<ScheduledTask>();
builder.Services.AddHostedService<SchedulerService>();


//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.Limits.MaxRequestBodySize = 1073741824;
//    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(8000);
//});

builder.WebHost.UseUrls("http://*:5000;http://localhost:5001");

PanelFilesManager.CheckPath();

builder.Services.AddScoped<IBaseRepository<TableElement>, TableElementsRepository>();
builder.Services.AddScoped<IBaseRepository<BaseTable>, BaseTableRepository>();
builder.Services.AddScoped<IBaseRepository<Account>, AccountRepository>();
builder.Services.AddScoped<IBaseRepository<FileModel>, FileModelRepository>();
builder.Services.AddScoped<ITableElementService, TableElementService>();
builder.Services.AddScoped<IBaseTableService, BaseTableService>();
builder.Services.AddScoped<IFileModelService, FileModelService>();
builder.Services.AddScoped<IAccountService, AccountService>();

var serviceProvider = builder.Services.BuildServiceProvider();
StaticDataHelper.Configure(serviceProvider);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<AdminSafeListMiddleware>(app.Configuration["AdminSafeList"]);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
