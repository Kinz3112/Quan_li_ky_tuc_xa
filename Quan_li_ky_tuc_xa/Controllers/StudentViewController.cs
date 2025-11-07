
using Quan_li_ky_tuc_xa.Models.Data;
using Quan_li_ky_tuc_xa.Models.Entities;
using Quan_li_ky_tuc_xa.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Quan_li_ky_tuc_xa.Filters;

namespace Quan_li_ky_tuc_xa.Controllers
{
    [AuthorizeByRole("Student")]
    [Route("Students")]
    public class StudentViewController : Controller
    {
        private readonly KTXContext _db;
        // Mỗi phòng tối đa 4 sinh viên
        private const int MAX_OCCUPANCY = 4;

        public StudentViewController(KTXContext context)
        {
            _db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Đăng ký phòng (hiển thị các phòng còn chỗ: current < MAX_OCCUPANCY)
        [HttpGet("DangKyPhong")]
        public async Task<IActionResult> DangKyPhong()
        {
            // Lấy username từ session hoặc từ identity
            string username = HttpContext.Session.GetString("Username") ?? User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Acc");

            // Các trạng thái hợp đồng được tính là đang chiếm chỗ
            var occupiedStatuses = new[] { "Đang ở", "Đã duyệt" };

            // Lấy tất cả phòng (include Toa để hiển thị tên tòa)
            var allRooms = await _db.Set<Phong>()
                .AsNoTracking()
                .Include(r => r.Toa)
                .ToListAsync();

            // Nếu không có phòng -> trả view rỗng
            if (allRooms == null || !allRooms.Any())
            {
                ViewBag.Rooms = new List<object>();
                ViewBag.RoomsJson = "[]";
                ViewBag.LoaiPhongList = new List<string>();
                ViewBag.ToaList = new List<string>();
                ViewBag.StudentExists = false;
                ViewBag.StudentJson = "null";
                ViewBag.DefaultDate = DateTime.Today;
                ViewBag.DefaultStatus = "Chờ duyệt";
                return View();
            }

            // Lấy số lượng Hop_dong theo MaPhong (chỉ đếm các trạng thái occupiedStatuses)
            var counts = await _db.Set<Hop_dong>()
                .AsNoTracking()
                .Where(h => !string.IsNullOrEmpty(h.MaPhong) && occupiedStatuses.Contains(h.LoaiHopDong))
                .GroupBy(h => h.MaPhong)
                .Select(g => new { MaPhong = g.Key, Count = g.Count() })
                .ToListAsync();

            var countDict = counts.ToDictionary(x => x.MaPhong!, x => x.Count, StringComparer.OrdinalIgnoreCase);

            // Build danh sách phòng còn chỗ (compute SoNguoi từ Hop_dong)
            var rooms = new List<object>();
            foreach (var r in allRooms)
            {
                // capacity (nếu có property SucChua dùng nó, không có -> fallback 4)
                int capacity = MAX_OCCUPANCY;
                try
                {
                    var prop = r.GetType().GetProperty("SucChua");
                    if (prop != null)
                    {
                        var val = prop.GetValue(r);
                        if (val != null && int.TryParse(val.ToString(), out var parsed)) capacity = parsed;
                    }
                }
                catch
                {
                    capacity = MAX_OCCUPANCY;
                }

                int current = 0;
                if (!string.IsNullOrEmpty(r.MaPhong) && countDict.ContainsKey(r.MaPhong))
                    current = countDict[r.MaPhong];

                // Nếu phòng chưa đầy -> đưa vào danh sách hiển thị
                if (current < capacity)
                {
                    rooms.Add(new
                    {
                        MaPhong = r.MaPhong,
                        TenPhong = r.Ten ?? r.MaPhong,
                        LoaiPhong = r.LoaiPhong,
                        Toa = r.Toa != null ? r.Toa.Ten : null,
                        SoNguoi = current,
                        SucChua = capacity
                    });
                }
            }

            // Danh sách loại phòng và tòa (dùng allRooms để giữ đầy đủ tập dữ liệu)
            var loaiPhongList = allRooms
                .Where(r => !string.IsNullOrEmpty(r.LoaiPhong))
                .Select(r => r.LoaiPhong!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var toaList = allRooms
                .Where(r => r.Toa != null && !string.IsNullOrEmpty(r.Toa.Ten))
                .Select(r => r.Toa!.Ten!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Serialize JSON giữ tiếng Việt (không escape unicode)
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            string roomsJson = JsonSerializer.Serialize(rooms, jsonOptions);

            ViewBag.Rooms = rooms;
            ViewBag.RoomsJson = roomsJson;
            ViewBag.LoaiPhongList = loaiPhongList;
            ViewBag.ToaList = toaList;

            // Prefill thông tin sinh viên (tìm theo Username)
            var sinhVien = await _db.Set<Sinh_Vien>()
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Username == username);

            if (sinhVien != null)
            {
                var stu = new
                {
                    MaSinhVien = sinhVien.MaSinhVien,
                    HoTen = sinhVien.HovaTen,
                    Email = sinhVien.User != null ? sinhVien.User.Email : null
                };
                ViewBag.StudentJson = JsonSerializer.Serialize(stu, jsonOptions);
                ViewBag.StudentExists = true;
            }
            else
            {
                // fallback lấy thông tin User nếu chưa có hồ sơ Sinh_Vien
                var user = await _db.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    var stu = new
                    {
                        Username = user.Username,
                        HoTen = user.Username,
                        Email = user.Email
                    };
                    ViewBag.StudentJson = JsonSerializer.Serialize(stu, jsonOptions);
                    ViewBag.StudentExists = false;
                }
                else
                {
                    ViewBag.StudentJson = "null";
                    ViewBag.StudentExists = false;
                }
            }

            ViewBag.DefaultDate = DateTime.Today;
            ViewBag.DefaultStatus = "Chờ duyệt";

            return View();
        }

        // POST: gửi yêu cầu đăng ký phòng
        // Form gửi field "MaPhong" (string)
        [HttpPost("DangKyPhong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyPhong(string MaPhong)
        {
            string username = HttpContext.Session.GetString("Username") ?? User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Acc");

            if (string.IsNullOrEmpty(MaPhong))
            {
                TempData["Error"] = "Vui lòng chọn phòng.";
                return RedirectToAction("DangKyPhong");
            }

            // Tìm phòng
            var room = await _db.Set<Phong>().FirstOrDefaultAsync(r => r.MaPhong == MaPhong);
            if (room == null)
            {
                TempData["Error"] = "Phòng không tồn tại.";
                return RedirectToAction("DangKyPhong");
            }

            // Tìm sinh viên
            var sinhVien = await _db.Set<Sinh_Vien>().FirstOrDefaultAsync(s => s.Username == username);
            if (sinhVien == null)
            {
                TempData["Error"] = "Bạn chưa có hồ sơ sinh viên. Vui lòng cập nhật thông tin trước khi đăng ký.";
                return RedirectToAction("DangKyPhong");
            }

            // Kiểm tra số người hiện tại bằng Hop_dong
            var occupiedStatuses = new[] { "Đang ở", "Đã duyệt" };
            int currentCount = await _db.Set<Hop_dong>()
                .CountAsync(h => h.MaPhong == MaPhong && occupiedStatuses.Contains(h.LoaiHopDong));

            int capacity = MAX_OCCUPANCY;
            try
            {
                var prop = room.GetType().GetProperty("SucChua");
                if (prop != null)
                {
                    var val = prop.GetValue(room);
                    if (val != null && int.TryParse(val.ToString(), out var parsed)) capacity = parsed;
                }
            }
            catch
            {
                capacity = MAX_OCCUPANCY;
            }

            if (currentCount >= capacity)
            {
                TempData["Error"] = "Phòng đã đầy hoặc không thể đăng ký.";
                return RedirectToAction("DangKyPhong");
            }

            // Kiểm tra đã có yêu cầu chờ duyệt chưa
            bool hasPending = await _db.Set<Hop_dong>().AnyAsync(h =>
                h.MaSinhVien == sinhVien.MaSinhVien &&
                h.MaPhong == MaPhong &&
                h.LoaiHopDong == "Chờ duyệt"
            );

            if (hasPending)
            {
                TempData["Error"] = "Bạn đã có yêu cầu đang chờ duyệt cho phòng này.";
                return RedirectToAction("DangKyPhong");
            }

            // Tạo Hop_dong mới
            var hopDong = new Hop_dong
            {
                MaHopDong = Guid.NewGuid().ToString(),
                MaSinhVien = sinhVien.MaSinhVien,
                MaNhanVienQuanLi = null,
                TenHopDong = "Đăng ký phòng",
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.MinValue,
                LoaiHopDong = "Chờ duyệt",
                MaPhong = MaPhong
            };

            try
            {
                _db.Set<Hop_dong>().Add(hopDong);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Gửi yêu cầu thành công. Vui lòng chờ quản lý duyệt.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Đã có lỗi khi gửi yêu cầu. Vui lòng thử lại.";
            }

            return RedirectToAction("DangKyPhong");
        }

        [HttpGet("MyRoom")]
        public async Task<IActionResult> PhongOHienTai()
        {
            string username = HttpContext.Session.GetString("Username") ?? User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Acc");

            var sinhVien = await _db.Set<Sinh_Vien>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Username == username);

            if (sinhVien == null)
            {
                TempData["Error"] = "Bạn chưa có hồ sơ sinh viên. Vui lòng cập nhật thông tin.";
                return View(Enumerable.Empty<RoomViewModel>());
            }

            var validStatuses = new[] { "Đang ở", "Đã duyệt" };

            var studentHops = await _db.Set<Hop_dong>()
                .AsNoTracking()
                .Where(h => h.MaSinhVien == sinhVien.MaSinhVien && validStatuses.Contains(h.LoaiHopDong))
                .Include(h => h.Phong)
                    .ThenInclude(p => p.Toa)
                .ToListAsync();

            if (!studentHops.Any())
            {
                TempData["DebugCount"] = 0;
                return View(Enumerable.Empty<RoomViewModel>());
            }

            var myRoomIds = studentHops
                .Where(h => !string.IsNullOrEmpty(h.MaPhong))
                .Select(h => h.MaPhong!)
                .Distinct()
                .ToList();

            var occupancy = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (myRoomIds.Any())
            {
                var allCounts = await _db.Set<Hop_dong>()
                    .AsNoTracking()
                    .Where(h => myRoomIds.Contains(h.MaPhong) && validStatuses.Contains(h.LoaiHopDong))
                    .GroupBy(h => h.MaPhong)
                    .Select(g => new { MaPhong = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var it in allCounts)
                {
                    if (!string.IsNullOrEmpty(it.MaPhong))
                        occupancy[it.MaPhong] = it.Count;
                }
            }

            var result = new List<RoomViewModel>();
            const int DEFAULT_CAPACITY = MAX_OCCUPANCY;

            foreach (var roomId in myRoomIds)
            {
                var hopForThis = studentHops.FirstOrDefault(h => h.MaPhong == roomId);
                var phong = hopForThis?.Phong;

                int soNguoi = occupancy.ContainsKey(roomId) ? occupancy[roomId] : (phong?.SoNguoi ?? 0);

                int sucChua = DEFAULT_CAPACITY;
                try
                {
                    if (phong != null)
                    {
                        var prop = phong.GetType().GetProperty("SucChua");
                        if (prop != null)
                        {
                            var val = prop.GetValue(phong);
                            if (val != null && int.TryParse(val.ToString(), out var parsed)) sucChua = parsed;
                        }
                    }
                }
                catch
                {
                    sucChua = DEFAULT_CAPACITY;
                }

                var allHopsForRoom = await _db.Set<Hop_dong>()
                    .AsNoTracking()
                    .Where(h => h.MaPhong == roomId && validStatuses.Contains(h.LoaiHopDong))
                    .ToListAsync();

                var hopInfos = allHopsForRoom.Select(h => new HopInfo
                {
                    MaHopDong = h.MaHopDong,
                    MaSinhVien = h.MaSinhVien,
                    LoaiHopDong = h.LoaiHopDong,
                    NgayBatDau = h.NgayBatDau,
                    NgayKetThuc = (DateTime?)(h.NgayKetThuc == DateTime.MinValue ? null : h.NgayKetThuc)
                }).ToList();

                var myHop = allHopsForRoom.FirstOrDefault(h => h.MaSinhVien == sinhVien.MaSinhVien);

                var vm = new RoomViewModel
                {
                    MaPhong = roomId,
                    Ten = phong?.Ten ?? phong?.MaPhong ?? roomId,
                    LoaiPhong = phong?.LoaiPhong,
                    ToaName = phong?.Toa != null ? (phong.Toa?.Ten ?? null) : null,
                    SoNguoi = soNguoi,
                    SucChua = sucChua,
                    HopInfos = hopInfos,
                    NgayNhan = myHop != null ? (DateTime?)myHop.NgayBatDau : null,
                    TrangThai = myHop != null ? myHop.LoaiHopDong : null
                };

                result.Add(vm);
            }

            ViewBag.DebugCount = result.Count;
            return View(result);
        }

        [HttpGet("HoSo")]
        public async Task<IActionResult> HoSoSinhVien()
        {
            string username = HttpContext.Session.GetString("Username") ?? User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Acc");

            var sinhVien = await _db.Set<Sinh_Vien>()
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Username == username);

            if (sinhVien == null)
            {
                ViewBag.Students = Enumerable.Empty<object>();
                TempData["Error"] = "Bạn chưa có hồ sơ sinh viên. Vui lòng cập nhật thông tin.";
                return View();
            }

            var validStatuses = new[] { "Đang ở", "Đã duyệt" };

            var myAllocations = await _db.Set<Hop_dong>()
                .AsNoTracking()
                .Where(p => p.MaSinhVien == sinhVien.MaSinhVien && validStatuses.Contains(p.LoaiHopDong))
                .ToListAsync();

            List<object> studentsList = new List<object>();

            if (myAllocations.Any())
            {
                var roomIds = myAllocations
                    .Where(p => !string.IsNullOrEmpty(p.MaPhong))
                    .Select(p => p.MaPhong)
                    .Distinct()
                    .ToList();

                var peers = await _db.Set<Hop_dong>()
                    .AsNoTracking()
                    .Where(p => !string.IsNullOrEmpty(p.MaPhong) && roomIds.Contains(p.MaPhong) && validStatuses.Contains(p.LoaiHopDong))
                    .Include(p => p.Sinh_Vien)
                        .ThenInclude(sv => sv.User)
                    .Select(p => new
                    {
                        MaSV = p.Sinh_Vien != null ? p.Sinh_Vien.MaSinhVien : null,
                        HoVaTen = p.Sinh_Vien != null ? p.Sinh_Vien.HovaTen : null,
                        NgaySinh = p.Sinh_Vien != null ? (DateTime?)p.Sinh_Vien.NgaySinh : null,
                        CCCD = (string?)null,
                        Lop = p.Sinh_Vien != null ? p.Sinh_Vien.Lop : null,
                        Khoa = p.Sinh_Vien != null ? p.Sinh_Vien.Khoa : null,
                        SDT = p.Sinh_Vien != null ? p.Sinh_Vien.Sdt : null,
                        Email = p.Sinh_Vien != null && p.Sinh_Vien.User != null ? p.Sinh_Vien.User.Email : null,
                        DiaChi = (string?)null,
                        PhongId = p.MaPhong
                    })
                    .ToListAsync();

                studentsList.AddRange(peers.Cast<object>());
            }
            else
            {
                studentsList.Add(new
                {
                    MaSV = sinhVien.MaSinhVien,
                    HoVaTen = sinhVien.HovaTen,
                    NgaySinh = (DateTime?)sinhVien.NgaySinh,
                    CCCD = (string?)null,
                    Lop = sinhVien.Lop,
                    Khoa = sinhVien.Khoa,
                    SDT = sinhVien.Sdt,
                    Email = sinhVien.User != null ? sinhVien.User.Email : null,
                    DiaChi = (string?)null,
                    PhongId = (string?)null
                });
            }

            ViewBag.Students = studentsList;

            ViewBag.Me = new
            {
                MaSV = sinhVien.MaSinhVien,
                HoVaTen = sinhVien.HovaTen,
                Username = sinhVien.Username
            };

            return View();
        }
    }
}
