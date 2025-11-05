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
        ///ROOM MANAGER
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

            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    data = phongs,
                    currentPage = page,
                    totalPages = totalPages
                });
            }

            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(await query.Take(pageSize).ToListAsync());
        }
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

            // ✅ Nếu ModelState HỢP LỆ thì mới update
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(phong);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.MaPhong))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(RManager));
            }

            // Nếu dữ liệu lỗi, load lại dropdown và trả về view
            ViewBag.MaToa = new SelectList(db.Toas, "MaToa", "Ten", phong.MaToa);
            ViewBag.MaTruongPhong = new SelectList(
                db.Sinh_Viens.Where(sv => sv.MaPhong == phong.MaPhong),
                "MaSinhVien", "HovaTen", phong.MaTruongPhong);

            return View(phong);
        }


        private bool PhongExists(string id)
        {
            return (db.Phongs?.Any(e => e.MaPhong == id)).GetValueOrDefault();
        }


        //Contract Manager
        public async Task<IActionResult> CManager(string? maHopDong, int page = 1)
        {
            int pageSize = 10;

            // 1️⃣ Tạo query cơ bản
            var query = db.Hop_Dongs
                .Include(h => h.Sinh_Vien)
                .Include(h => h.Nhan_Vien)
                .Include(h => h.Phong)
                .AsQueryable();

            // 2️⃣ Nếu có từ khóa tìm kiếm (mã hợp đồng)
            if (!string.IsNullOrEmpty(maHopDong))
            {
                query = query.Where(h => h.MaHopDong.Contains(maHopDong));
            }

            // 3️⃣ Đếm tổng số bản ghi
            var totalHopDong = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalHopDong / (double)pageSize);

            // 4️⃣ Lấy dữ liệu trang hiện tại
            var hopDongs = await query
                .OrderBy(h => h.MaHopDong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new
                {
                    h.MaHopDong,
                    h.TenHopDong,
                    h.LoaiHopDong,
                    h.NgayBatDau,
                    h.NgayKetThuc,
                    SinhVien = h.Sinh_Vien != null ? h.Sinh_Vien.HovaTen : "Không có",
                    NhanVien = h.Nhan_Vien != null ? h.Nhan_Vien.Ten : "Không có",
                    Phong = h.Phong != null ? h.Phong.Ten : "Không có"
                })
                .ToListAsync();

            // 5️⃣ Trả về JSON nếu là AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    data = hopDongs,
                    currentPage = page,
                    totalPages = totalPages
                });
            }

            // 6️⃣ Nếu là request thường → trả về View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchKeyword = maHopDong;

            return View(await query
                .OrderBy(h => h.MaHopDong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync());
        }

        public IActionResult Edit_Contract(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // ✅ Include các bảng liên quan để tránh null reference trong View
            var hopdong = db.Hop_Dongs
                .Include(h => h.Sinh_Vien)
                .Include(h => h.Nhan_Vien)
                .Include(h => h.Phong)
                .FirstOrDefault(h => h.MaHopDong == id);

            // ✅ Nếu không tìm thấy hợp đồng → trả về 404
            if (hopdong == null)
            {
                return NotFound();
            }

            // ✅ Luôn đảm bảo ViewBag có dữ liệu, tránh null
            //ViewBag.MaSinhVien = new SelectList(db.Sinh_Viens?.ToList() ?? new List<Sinh_Vien>(), "MaSinhVien", "HovaTen", hopdong.MaSinhVien);
            //ViewBag.MaNhanVien = new SelectList(db.Nhan_Viens?.ToList() ?? new List<Nhan_vien>(), "MaNhanVien", "HovaTen", hopdong.MaNhanVienQuanLi);
            //ViewBag.MaPhong = new SelectList(db.Phongs?.ToList() ?? new List<Phong>(), "MaPhong", "Ten", hopdong.MaPhong);
            ViewBag.MaSinhVien = new SelectList(db.Sinh_Viens.Where(sv => sv.MaSinhVien == hopdong.MaSinhVien), "MaSinhVien", "HovaTen", hopdong.MaSinhVien);
            ViewBag.MaNhanVienQuanLi = new SelectList(db.Nhan_Viens.Where(sv => sv.MaNhanVien == hopdong.MaNhanVienQuanLi), "MaNhanVien", "Ten", hopdong.MaNhanVienQuanLi);
            ViewBag.MaPhong= new SelectList(db.Phongs.Where(sv => sv.MaPhong == hopdong.MaPhong), "MaPhong", "Ten", hopdong.MaPhong);




            return View(hopdong);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Contract(
             string id,
            [Bind("MaHopDong,MaNhanVienQuanLi,MaSinhVien,TenHopDong,NgayBatDau,NgayKetThuc,LoaiHopDong,MaPhong")]
            Hop_dong hopdong)
        {
            if (id != hopdong.MaHopDong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(hopdong);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(CManager));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Hop_Dongs.Any(h => h.MaHopDong == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu có lỗi, load lại dropdown
            ViewBag.MaSinhVien = new SelectList(db.Sinh_Viens, "MaSinhVien", "HovaTen", hopdong.MaSinhVien);
            ViewBag.MaNhanVien = new SelectList(db.Nhan_Viens, "MaNhanVien", "HovaTen", hopdong.MaNhanVienQuanLi);
            ViewBag.MaPhong = new SelectList(db.Phongs, "MaPhong", "Ten", hopdong.MaPhong);

            return View(hopdong);
        }

    }

}

