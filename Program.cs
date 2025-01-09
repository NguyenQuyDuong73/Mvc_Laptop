using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MvcLaptopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MvcLaptopContext") ?? throw new InvalidOperationException("Connection string 'MvcLaptopContext' not found.")));
}
builder.Services.AddDistributedMemoryCache();  // Sử dụng bộ nhớ để lưu trữ session

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILaptopService, LaptopService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;  // Cookie chỉ có thể truy cập từ server
    options.Cookie.IsEssential = true;  // Làm cho cookie session là cần thiết
});
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<MvcLaptopContext>()
    .AddDefaultTokenProviders();

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MvcLaptopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MvcLaptopContext")));
}

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.Cookie.HttpOnly = true;
//     options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

//     options.LoginPath = "/Account/Login";
//     options.AccessDeniedPath = "/Account/AccessDenied";
//     options.SlidingExpiration = true;
// });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
options.LoginPath = "/Account/Login"; 
options.AccessDeniedPath = "/Account/AccessDenied"; 
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
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
