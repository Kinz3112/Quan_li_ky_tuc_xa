using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Đăng ký DbContext trước khi build
builder.Services.AddDbContext<KTXContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KTXConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// ✅ Chạy khởi tạo dữ liệu (nếu có DbInitializer)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //DbInitializer.Initialize(services);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acc}/{action=Login}/{id?}");

app.Run();
