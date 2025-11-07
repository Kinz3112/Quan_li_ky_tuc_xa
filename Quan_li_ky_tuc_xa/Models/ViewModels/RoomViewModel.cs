using Microsoft.AspNetCore.Mvc;

namespace Quan_li_ky_tuc_xa.Models.ViewModels
{
    public class HopInfo
    {
        public string? MaHopDong { get; set; }
        public string? MaSinhVien { get; set; }
        public string? LoaiHopDong { get; set; } // ví dụ "Đang ở", "Chờ duyệt"
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }

    public class RoomViewModel
    {
        // Thông tin phòng
        public string? MaPhong { get; set; }
        public string? Ten { get; set; }
        public string? LoaiPhong { get; set; }
        public string? ToaName { get; set; }

        // Số người hiện tại & sức chứa
        public int SoNguoi { get; set; }
        public int SucChua { get; set; }

        // Thông tin hợp đồng (dành cho sinh viên hiện tại hoặc tất cả hợp đồng trong phòng)
        public List<HopInfo> HopInfos { get; set; } = new List<HopInfo>();

        // Trạng thái/ngày nhận cho sinh viên hiện tại (nếu cần)
        public DateTime? NgayNhan { get; set; }
        public string? TrangThai { get; set; } // trạng thái hợp đồng chính của sinh viên ở phòng này
    }
}
