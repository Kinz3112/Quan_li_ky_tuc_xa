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

            // 🔹 Include Toa, Truong_phong, và Sinh_Viens để đếm số người
            var query = db.Phongs
                .Include(p => p.Truong_phong)
                .Include(p => p.Toa)
                .Include(p => p.Sinh_Viens) // cần có navigation này trong model Phong
                .AsQueryable();

            // 🔹 Lọc theo số người (đếm từ danh sách sinh viên)
            if (soNguoi.HasValue)
            {
                query = query.Where(p => p.Sinh_Viens.Count == soNguoi.Value);
            }

            var totalPhong = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPhong / (double)pageSize);

            // 🔹 Lấy dữ liệu trang hiện tại
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
                    SoNguoi = p.Sinh_Viens.Count, // ✅ Tự động đếm số sinh viên trong phòng
                    TruongPhong = p.Truong_phong != null ? p.Truong_phong.HovaTen : "Không có"
                })
                .ToListAsync();

            // 🔹 Nếu là AJAX request → trả về JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    data = phongs,
                    currentPage = page,
                    totalPages = totalPages
                });
            }

            // 🔹 Nếu là request thông thường → trả View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Dùng Select tương tự để View hiển thị thống nhất
            var phongsList = await query
                .OrderBy(p => p.MaPhong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(phongsList);
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

            ViewBag.MaTruongPhong = new SelectList(db.Sinh_Viens.Where(sv => sv.MaPhong == phong.MaPhong), "MaSinhVien", "HovaTen", phong.MaTruongPhong);

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

            //ViewBag.MaSinhVien = new SelectList(db.Sinh_Viens.Where(sv => sv.MaSinhVien == hopdong.MaSinhVien), "MaSinhVien", "HovaTen", hopdong.MaSinhVien);
            //ViewBag.MaNhanVienQuanLi = new SelectList(db.Nhan_Viens.Where(sv => sv.MaNhanVien == hopdong.MaNhanVienQuanLi), "MaNhanVien", "Ten", hopdong.MaNhanVienQuanLi);
            //ViewBag.MaPhong= new SelectList(db.Phongs.Where(sv => sv.MaPhong == hopdong.MaPhong), "MaPhong", "Ten", hopdong.MaPhong);

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
            return View(hopdong);
        }


        // GET: Employees/Delete_Contract/id
        public IActionResult Delete_Contract(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var hopdong = db.Hop_Dongs
                .Include(h => h.Sinh_Vien)
                .Include(h => h.Nhan_Vien)
                .Include(h => h.Phong)
                .FirstOrDefault(h => h.MaHopDong == id);

            if (hopdong == null)
            {
                return NotFound();
            }

            return View(hopdong);
        }
        [HttpPost, ActionName("Delete_Contract")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // 1️⃣ Tìm hợp đồng theo mã
            var hopdong = await db.Hop_Dongs
                .Include(h => h.Hoa_Don)
                .Include(h => h.Sinh_Vien)
                .FirstOrDefaultAsync(h => h.MaHopDong == id);

            if (hopdong == null)
                return NotFound();

            // 2️⃣ Kiểm tra hóa đơn liên quan
            if (hopdong.Hoa_Don != null)
            {
                if (hopdong.Hoa_Don.TrangThai == true)
                {
                    // 3️⃣ Xóa hóa đơn
                    db.Hoa_Dons.Remove(hopdong.Hoa_Don);

                    // 4️⃣ Xóa sinh viên thuộc hợp đồng
                    if (hopdong.Sinh_Vien != null)
                    {
                        db.Sinh_Viens.Remove(hopdong.Sinh_Vien);
                    }

                    // 5️⃣ Xóa hợp đồng
                    db.Hop_Dongs.Remove(hopdong);

                    // 6️⃣ Lưu thay đổi
                    await db.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã xóa hợp đồng, hóa đơn và sinh viên thành công.";
                    return RedirectToAction(nameof(CManager));
                }
                else
                {
                    // ❌ Hóa đơn chưa thanh toán
                    TempData["ErrorMessage"] = "Không thể xóa hợp đồng vì hóa đơn chưa thanh toán (TrangThai = false).";
                    return RedirectToAction(nameof(CManager));
                }
            }
            else
            {
                // ❌ Không có hóa đơn
                TempData["ErrorMessage"] = "Hợp đồng này chưa có hóa đơn, không thể xóa.";
                return RedirectToAction(nameof(CManager));
            }
        }

        //Invoice MANAGER
        public async Task<IActionResult> InvoiceManager(bool? trangThai, int page = 1)
        {
            int pageSize = 10;

            // 1️⃣ Tạo query
            var query = db.Hoa_Dons
                .Include(h => h.Hop_Dong)
                .ThenInclude(hd => hd.Sinh_Vien)
                .AsQueryable();

            // 2️⃣ Lọc theo trạng thái nếu có
            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            // 3️⃣ Đếm tổng số bản ghi
            var totalHoaDon = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalHoaDon / (double)pageSize);

            // 4️⃣ Lấy dữ liệu phân trang
            var hoaDons = await query
                .OrderByDescending(h => h.NgayLap) // sắp xếp mới nhất lên đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 5️⃣ Gửi dữ liệu sang View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TrangThai = trangThai;

            return View(hoaDons);
        }
        
        public async Task<IActionResult> Edit_HoaDon(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var hoadon = await db.Hoa_Dons
                .Include(h => h.Hop_Dong)
                .ThenInclude(hd => hd.Sinh_Vien)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoadon == null)
                return NotFound();

            // Truyền thêm danh sách hợp đồng nếu muốn thay đổi
            ViewBag.HopDongList = db.Hop_Dongs
                .Select(h => new { h.MaHopDong, Ten = h.TenHopDong })
                .ToList();

            return View(hoadon);
        }

        // ✅ Xử lý khi bấm "Lưu"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_HoaDon(string id, Hoa_don model)
        {
            if (id != model.MaHoaDon)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var hoadon = await db.Hoa_Dons.FindAsync(id);
                    if (hoadon == null)
                        return NotFound();

                    // Cập nhật thông tin
                    hoadon.TongTien = model.TongTien;
                    hoadon.TrangThai = model.TrangThai;
                    hoadon.NgayLap = model.NgayLap;

                    // Nếu hóa đơn đã thanh toán → cập nhật NgàyThanhToan
                    if (model.TrangThai)
                    {
                        hoadon.NgayThanhToan = DateTime.Now;
                    }
                    else
                    {
                        hoadon.NgayThanhToan = DateTime.MinValue; // hoặc để null nếu bạn cho phép null
                    }

                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật hóa đơn thành công!";
                    return RedirectToAction(nameof(InvoiceManager));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                }
            }

            ViewBag.HopDongList = db.Hop_Dongs
                .Select(h => new { h.MaHopDong, Ten = h.TenHopDong })
                .ToList();

            return View(model);
        }

    }
}
