using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Entities;

namespace Quan_li_ky_tuc_xa.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new KTXContext(serviceProvider.GetRequiredService<DbContextOptions<KTXContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.Sinh_Viens.Any())
                {
                    return;
                }

                var nhanViens = new Nhan_vien[]
                {
                    new Nhan_vien
                    {
                        MaNhanVien = "NV001",
                        Ten = "Nguyễn Văn Hùng",
                        ChucVu = "Giám đốc",
                        Sdt = "0905123456",
                        GioiTinh = true,
                        NgaySinh = DateTime.Parse("1980-02-15"),
                        
                    },
                    new Nhan_vien
                    {
                        MaNhanVien = "NV002",
                        Ten = "Trần Thị Lan",
                        ChucVu = "Phó giám đốc",
                        Sdt = "0912345678",
                        GioiTinh = false,
                        NgaySinh = DateTime.Parse("1985-07-20"),
                        
                    },
                    new Nhan_vien
                    {
                        MaNhanVien = "NV003",
                        Ten = "Lê Minh Tuấn",
                        ChucVu = "Nhân viên hành chính",
                        Sdt = "0934567890",
                        GioiTinh = true,
                        NgaySinh = DateTime.Parse("1990-04-12"),
                       
                    },
                    new Nhan_vien
                    {
                        MaNhanVien = "NV004",
                        Ten = "Phạm Thị Hoa",
                        ChucVu = "Nhân viên kỹ thuật",
                        Sdt = "0987123456",
                        GioiTinh = false,
                        NgaySinh = DateTime.Parse("1993-11-08"),
                       
                    }
                };
                foreach (var nv in nhanViens)
                {
                    context.Nhan_Viens.Add(nv);
                }
                context.SaveChanges();

                var toas = new Toa[]
                {
                    new Toa
                    {
                        MaToa = "T001",
                        Ten = "Tòa A",
                        MaNhanVienQuanLi = "NV002"
                    },
                    new Toa
                    {
                        MaToa = "T002",
                        Ten = "Tòa B",
                        MaNhanVienQuanLi = "NV003"
                    },
                    new Toa
                    {
                        MaToa = "T003",
                        Ten = "Tòa C",
                        MaNhanVienQuanLi = "NV004"
                    }
                };
                foreach (var toa in toas)
                {
                    context.Toas.Add(toa);
                }
                context.SaveChanges();

                var phongs = new Phong[]
                {
                    new Phong
                    {
                        MaPhong = "P101",
                        Ten = "Phòng 101",
                        MaTruongPhong = "SV001", // sinh viên trưởng phòng (có thể gán sau khi có dữ liệu Sinh_Vien)
                        MaToa = "T001",
                        LoaiPhong = "Nam",
                        SoNguoi = 3
                    },
                    new Phong
                    {
                        MaPhong = "P201",
                        Ten = "Phòng 201",
                        MaTruongPhong = "SV002",
                        MaToa = "T002",
                        LoaiPhong = "Nữ",
                        SoNguoi = 2
                    },
                    new Phong
                    {
                        MaPhong = "P301",
                        Ten = "Phòng 301",
                        MaTruongPhong = "SV003",
                        MaToa = "T003",
                        LoaiPhong = "Nam",
                        SoNguoi = 4
                    }
                };
                foreach (var p in phongs)
                {
                    context.Phongs.Add(p);
                }
                context.SaveChanges();

                var sinhViens = new Sinh_Vien[]
                {
                    new Sinh_Vien
                    {
                        MaSinhVien = "SV001",
                        HovaTen = "Nguyễn Văn An",
                        Lop = "CTK45",
                        Khoa = "Công nghệ thông tin",
                        Sdt = "0912345678",
                        GioiTinh = true,
                        MaPhong = "P101",
                        NgaySinh = DateTime.Parse("2003-05-12")
                    },
                    new Sinh_Vien
                    {
                        MaSinhVien = "SV002",
                        HovaTen = "Trần Thị Bình",
                        Lop = "KTK44",
                        Khoa = "Kinh tế",
                        Sdt = "0987654321",
                        GioiTinh = false,
                        MaPhong = "P101",
                        NgaySinh = DateTime.Parse("2002-11-25")
                    },
                    new Sinh_Vien
                    {
                        MaSinhVien = "SV003",
                        HovaTen = "Lê Minh Cường",
                        Lop = "QTKD46",
                        Khoa = "Quản trị kinh doanh",
                        Sdt = "0978123456",
                        GioiTinh = true,
                        MaPhong = "P201",
                        NgaySinh = DateTime.Parse("2004-03-08")
                    },
                    new Sinh_Vien
                    {
                        MaSinhVien = "SV004",
                        HovaTen = "Phạm Thị Dung",
                        Lop = "CK47",
                        Khoa = "Cơ khí",
                        Sdt = "0934567890",
                        GioiTinh = false,
                        MaPhong = "P301",
                        NgaySinh = DateTime.Parse("2005-07-20")
                    }
                };
                foreach (var major in sinhViens)
                {
                    context.Sinh_Viens.Add(major);
                }
                context.SaveChanges();

                var dichVus = new Dich_vu[]
                {
                    new Dich_vu
                    {
                        MaDichVu = "DV001",
                        TenDichVu = "Điện nước",
                        NhaCungCap = "Công ty Điện lực TP.HCM"
                    },
                    new Dich_vu
                    {
                        MaDichVu = "DV002",
                        TenDichVu = "Internet",
                        NhaCungCap = "VNPT"
                    },
                    new Dich_vu
                    {
                        MaDichVu = "DV003",
                        TenDichVu = "Giặt ủi",
                        NhaCungCap = "Dịch vụ Giặt Ủi Sinh Viên"
                    },
                    new Dich_vu
                    {
                        MaDichVu = "DV004",
                        TenDichVu = "Thuê phòng",
                        NhaCungCap = "Ban quản lí ký túc xá"
                    }
                };
                foreach (var dv in dichVus)
                {
                    context.Dich_Vus.Add(dv);
                }
                context.SaveChanges();

                var hopDongs = new Hop_dong[]
            {
                new Hop_dong
                {
                    MaHopDong = "HD001",
                    MaNhanVienQuanLi = "NV002",
                    MaSinhVien = "SV001",
                    TenHopDong = "Hợp đồng thuê phòng 101",
                    NgayBatDau = DateTime.Parse("2025-01-01"),
                    NgayKetThuc = DateTime.Parse("2025-12-31"),
                    LoaiHopDong = "Thuê phòng",
                    MaDichVu = "DV004"
                },
                new Hop_dong
                {
                    MaHopDong = "HD002",
                    MaNhanVienQuanLi = "NV003",
                    MaSinhVien = "SV002",
                    TenHopDong = "Hợp đồng thuê phòng 201",
                    NgayBatDau = DateTime.Parse("2025-03-01"),
                    NgayKetThuc = DateTime.Parse("2026-02-28"),
                    LoaiHopDong = "Thuê phòng",
                    MaDichVu = "DV004"
                },
                new Hop_dong
                {
                    MaHopDong = "HD003",
                    MaNhanVienQuanLi = "NV004",
                    MaSinhVien = "SV003",
                    TenHopDong = "Hợp đồng thuê phòng 301",
                    NgayBatDau = DateTime.Parse("2025-06-01"),
                    NgayKetThuc = DateTime.Parse("2026-05-31"),
                    LoaiHopDong = "Thuê phòng",
                    MaDichVu = "DV004"
                }
            };
                foreach (var hd in hopDongs)
                {
                    context.Hop_Dongs.Add(hd);
                }
                context.SaveChanges();

                var hoaDons = new List<Hoa_don>
            {
                new Hoa_don
                {
                    MaHoaDon = "HĐ001",
                    TongTien = 1200000,
                    NgayLap = new DateTime(2025, 10, 1),
                    NgayThanhToan = new DateTime(2025, 10, 5),
                    TrangThai = true,
                    MaHopDong = "HD001"
                },
                new Hoa_don
                {
                    MaHoaDon = "HĐ002",
                    TongTien = 900000,
                    NgayLap = new DateTime(2025, 10, 2),
                    TrangThai = false,
                    MaHopDong = "HD002"
                },
                new Hoa_don
                {
                    MaHoaDon = "HĐ003",
                    TongTien = 1500000,
                    NgayLap = new DateTime(2025, 10, 3),
                    NgayThanhToan = new DateTime(2025, 10, 6),
                    TrangThai = true,
                    MaHopDong = "HD003"
                }
            };
            foreach (var h in hoaDons)
            {
                context.Hoa_Dons.Add(h);
            }
            context.SaveChanges();


                var users = new List<User>
            {
                new User
                {
                    Username = "quanli01",
                    Password = "123456",
                    
                },
                new User
                {
                    Username = "nhanvien01",
                    Password = "123456",
                    
                },
                new User
                {
                    Username = "sinhvien01",
                    Password = "123456",
                    
                }
            };

            foreach (var h in users)
            {
                context.Users.Add(h);
            }
            context.SaveChanges();
            }
        }
    }
}
