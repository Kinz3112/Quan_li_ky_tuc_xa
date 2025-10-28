namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Hop_dong
    {
        public Hop_dong() { 
        }
        public string MaHopDong { get; set; }
        public string MaNhanVienQuanLi { get; set; }
        public string MaSinhVien {  get; set; }
        public string TenHopDong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc {  get; set; }
        public string LoaiHopDong { get; set; }
        public string MaDichVu {  get; set; }
        public string MaPhong {  get; set; }
        public virtual Nhan_vien Nhan_Vien { get; set; }
        public virtual Sinh_Vien Sinh_Vien { get;set; }
        public virtual Phong Phong {  get; set; }
        public virtual Dich_vu Dich_Vu { get; set; }
        public virtual Hoa_don Hoa_Don { get; set; }
    }
}
