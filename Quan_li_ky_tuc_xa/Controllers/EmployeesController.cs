using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Data;
using Quan_li_ky_tuc_xa.Models.Entities;
using System.Threading.Tasks;

namespace Quan_li_ky_tuc_xa.Controllers
{
    public class EmployeesController : Controller
    {
        private KTXContext db;
        public EmployeesController(KTXContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RManager(int? soNguoi, int page = 1)
        {
            int pageSize = 10;

            var query = db.Phongs
                .Include(p => p.Truong_phong)
                .Include(p => p.Toa)
                .AsQueryable();

            if (soNguoi.HasValue)
            {
                query = query.Where(p => p.SoNguoi == soNguoi.Value);
            }

            var totalPhong = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPhong / (double)pageSize);

            var phongs = await query
                .OrderBy(p => p.MaPhong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.MaPhong,
                    p.Ten,
                    Toa = p.Toa.Ten,
                    p.LoaiPhong,
                    p.SoNguoi,
                    TruongPhong = p.Truong_phong != null ? p.Truong_phong.HovaTen : "Không có"
                })
                .ToListAsync();

            // 🔹 Nếu là AJAX thì trả về JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    data = phongs,
                    currentPage = page,
                    totalPages = totalPages
                });
            }

            // 🔹 Lần đầu vào trang thì render bình thường
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(await query.Take(pageSize).ToListAsync());
        }


        //public async Task<IActionResult> RManager(int? soNguoi, int page = 1)
        //{
        //    int pageSize = 10;

        //    var query = db.Phongs
        //        .Include(a => a.Truong_phong)
        //        .Include(a => a.Toa)
        //        .AsQueryable();

        //    // 🔹 Lọc theo số người
        //    if (soNguoi.HasValue)
        //    {
        //        query = query.Where(p => p.SoNguoi == soNguoi.Value);
        //    }

        //    var totalPhong = await query.CountAsync();
        //    var totalPages = (int)Math.Ceiling(totalPhong / (double)pageSize);

        //    var phongs = await query
        //        .OrderBy(p => p.MaPhong)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.SoNguoi = soNguoi;

        //    // Nếu request là AJAX → chỉ trả về partial
        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        return PartialView("_PhongListPartial", phongs);
        //    }

        //    // Lần đầu vào trang → view đầy đủ
        //    return View(phongs);
        //}

        public IActionResult Edit_Phong(string id)
        {
            if (id == null || db.Phongs == null)
            {
                return NotFound();
            }

            var phong = db.Phongs
                .Include(p => p.Toa)
                .Include(p => p.Truong_phong)
                .FirstOrDefault(p => p.MaPhong == id);

            if (phong == null)
            {
                return NotFound();
            }

            ViewBag.MaTruongPhong = new SelectList(db.Sinh_Viens.Where(sv => sv.MaPhong == phong.MaPhong),"MaSinhVien","HovaTen",phong.MaTruongPhong);

            return View(phong);
        }

        // POST: Phong/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Phong(string id, [Bind("MaPhong,Ten,MaToa,LoaiPhong,SoNguoi,MaTruongPhong")] Phong phong)
        {
            if (id != phong.MaPhong)
                return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    db.Update(phong);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.MaPhong))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(RManager));
            }

            var existingPhong = await db.Phongs.FirstOrDefaultAsync(p => p.MaPhong == id);
            if (existingPhong == null)
                return NotFound();

            ViewBag.MaToa = new SelectList(db.Toas, "MaToa", "Ten", phong.MaToa);
            ViewBag.MaTruongPhong = new SelectList(db.Sinh_Viens, "MaSinhVien", "HovaTen", phong.MaTruongPhong);
            return View(phong);

            
        }

        private bool PhongExists(string id)
        {
            return (db.Phongs?.Any(e => e.MaPhong == id)).GetValueOrDefault();
        }

    }
}

