namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Dich_vu
    {
        public Dich_vu() {
        }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string NhaCungCap { get; set; }
        public virtual Hop_dong Hop_Dong { get; set; }
    }
}
