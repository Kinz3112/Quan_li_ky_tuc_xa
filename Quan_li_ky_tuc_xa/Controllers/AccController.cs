using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Data;
using Quan_li_ky_tuc_xa.Models.ViewModels;
using Quan_li_ky_tuc_xa.Models.Entities;

namespace Quan_li_ky_tuc_xa.Controllers
{
    [Route("Acc")]
    public class AccController : Controller
    {
        private readonly KTXContext _db;

        public AccController(KTXContext context)
        {
            _db = context;
        }

        [HttpGet("Login")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Title"] = "Đăng Nhập";
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Dữ liệu nhập không hợp lệ.";
                return View(model);
            }

            // Lấy user, include Role + navigation tới Sinh_Vien / Nhan_Vien
            var user = await _db.Set<User>()
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Sinh_Vien)
                .Include(u => u.Nhan_Vien)
                .FirstOrDefaultAsync(u => u.Username == model.TenDangNhap && u.isActive == true);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View(model);
            }

            // Kiểm tra mật khẩu (nếu mã hóa thì đổi logic tương ứng)
            if (!string.Equals(model.Password, user.Password, StringComparison.Ordinal))
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View(model);
            }

            // --- XÁC ĐỊNH VAI TRÒ (CHỈNH THEO YÊU CẦU) ---
            // Yêu cầu: các Role.Id = 1..6 (liên kết với nv01..nv06 / Nhan_Vien) => đẩy vào trang quản lý nhân viên.
            string roleName = null;
            var role = user.Role;
            int? roleId = role?.Id;
            string roleDbName = role?.RoleName?.Trim();

            // Quy tắc ưu tiên:
            // 1) Nếu roleId trong 1..6 và user là Nhan_Vien => Manager
            if (roleId.HasValue && roleId.Value >= 1 && roleId.Value <= 6 && user.Nhan_Vien != null)
            {
                roleName = "Manager";
            }
            else if (!string.IsNullOrEmpty(roleDbName))
            {
                // 2) Nếu RoleName là 'NhanVien'/'Staff'/'Admin' -> Manager
                if (roleDbName.Equals("NhanVien", StringComparison.OrdinalIgnoreCase)
                    || roleDbName.Equals("Manager", StringComparison.OrdinalIgnoreCase)
                    || roleDbName.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    || roleDbName.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    // Nếu roleDbName = "NhanVien", coi là Manager
                    roleName = "Manager";
                }
                // 3) Nếu RoleName là 'SinhVien' -> Student
                else if (roleDbName.Equals("SinhVien", StringComparison.OrdinalIgnoreCase)
                         || roleDbName.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    roleName = "Student";
                }
                else
                {
                    // Các role khác: giữ nguyên tên từ DB (fallback)
                    roleName = roleDbName;
                }
            }
            else
            {
                // 4) Fallback: suy đoán từ navigation properties
                if (user.Nhan_Vien != null) roleName = "Manager";
                else if (user.Sinh_Vien != null) roleName = "Student";
            }

            if (string.IsNullOrEmpty(roleName))
            {
                ViewBag.Error = "Tài khoản chưa được gán vai trò. Liên hệ quản trị viên.";
                return View(model);
            }

            bool isManager = roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase);
            bool isStudent = roleName.Equals("Student", StringComparison.OrdinalIgnoreCase);

            if (!isManager && !isStudent)
            {
                ViewBag.Error = "Tài khoản không có quyền truy cập.";
                return View(model);
            }

            // Cập nhật Last_At (attach để update vì AsNoTracking trước đó)
            try
            {
                var tracked = await _db.Set<User>().FirstOrDefaultAsync(u => u.Username == user.Username);
                if (tracked != null)
                {
                    tracked.Last_At = DateTime.Now;
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                // ignore update failure
            }

            // Tạo claims và sign-in cookie
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName) // roleName là "Manager" hoặc "Student"
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            // Lưu session keys
            HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
            HttpContext.Session.SetString("TenDangNhap", user.Username ?? string.Empty);
            HttpContext.Session.SetString("Role", roleName);
            HttpContext.Session.SetString("VaiTroIds", roleName);

            // Chuyển hướng
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return isManager
                ? RedirectToAction("Index", "Employees")
                : RedirectToAction("Index", "StudentView");
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acc");
        }

        [HttpGet("AccessDenied")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewBag.Message = "Bạn không có quyền truy cập vào trang này.";
            return View();
        }
    }
}
