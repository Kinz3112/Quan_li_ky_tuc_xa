using Microsoft.AspNetCore.Mvc;

namespace Quan_li_ky_tuc_xa.Models.ViewModels
{
    public class RequestItemDto
    {
        public string MaHopDong { get; set; } = "";
        public string? MaSinhVien { get; set; }
        public string? SinhVienHoTen { get; set; }
        public string? MaPhong { get; set; }
        public string? TenPhong { get; set; }
        public DateTime? NgayPhanBo { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }

    public class ManageRoomsViewModel
    {
        public IEnumerable<Quan_li_ky_tuc_xa.Models.Entities.Phong> Rooms { get; set; } = new List<Quan_li_ky_tuc_xa.Models.Entities.Phong>();
        public IEnumerable<RequestItemDto> RequestsDto { get; set; } = new List<RequestItemDto>();
    }

    // DTO dùng để render peers partial
    public class PeerDto
    {
        public string? MaSinhVien { get; set; }
        public string? HoVaTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? TrangThai { get; set; } // trạng thái hợp đồng
    }
}
