namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Hoa_don
    {
        public string? MaHoaDon { get; set; }
        public float TongTien { get; set; }
        public DateTime NgayLap { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public bool TrangThai { get; set; }//true la da thanh toan, flase la chua
        public string? MaHopDong {  get; set; }
        public virtual Hop_dong? Hop_Dong { get; set; }
    }
}
