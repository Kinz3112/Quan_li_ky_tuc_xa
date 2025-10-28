using Microsoft.AspNetCore.Mvc;
using Quan_li_ky_tuc_xa.Models.Data;
using Quan_li_ky_tuc_xa.Models.Entities;
using System.Linq;

namespace Quan_li_ky_tuc_xa.Controllers
{
    public class AccController : Controller
    {
        private readonly KTXContext db;

        public AccController(KTXContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Sign In";
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username, Password")] User use)
        {
            //var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username).FirstOrDefault(l => l.Password == user.Password;

            //if (existingUser != null && BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            //{
            //    // ✅ Đăng nhập thành công
            //    HttpContext.Session.SetString("Username", existingUser.Username);
            //    TempData["Success"] = "Đăng nhập thành công!";
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // ❌ Sai username hoặc password
            //    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            //    return View(user);
            //}
            //if (id == null || db.Learners == null)
            //{
            //    return NotFound();
            //}

            var nv = db.Nhan_Viens.Find(use.Username);

            if (nv != null)
            {
                var c = db.Users.FirstOrDefault(u => u.Username == use.Username && u.Password == use.Password);
                return RedirectToAction("Index", "Employees");
            }
            return View();
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Kiểm tra trùng username
            var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View(user);
            }

            // 🔐 Hash mật khẩu trước khi lưu
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            db.Users.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}
