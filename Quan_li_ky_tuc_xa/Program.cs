using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<KTXContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KTXConnection")));

// MVC
builder.Services.AddControllersWithViews();

// Session & cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acc/Login";
        options.LogoutPath = "/Acc/Logout";
        options.AccessDeniedPath = "/Acc/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "KTXAuth";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// optional DB init
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // DbInitializer.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();         // session first
app.UseAuthentication();  // then authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


