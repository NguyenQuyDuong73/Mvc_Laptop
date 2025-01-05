using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MvcLaptopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcLaptopContext") ?? throw new InvalidOperationException("Connection string 'MvcLaptopContext' not found.")));

builder.Services.AddDistributedMemoryCache();  // Sử dụng bộ nhớ để lưu trữ session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;  // Cookie chỉ có thể truy cập từ server
    options.Cookie.IsEssential = true;  // Làm cho cookie session là cần thiết
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ILaptopService, LaptopService>();

builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHttpContextAccessor();

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MvcLaptopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MvcLaptopContext")));
}
// else
// {
//     builder.Services.AddDbContext<MvcLaptopContext>(options =>
//         options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionMvcLaptopContext")));
// }
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
