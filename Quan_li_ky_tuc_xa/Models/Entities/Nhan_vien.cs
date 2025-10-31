using System.Collections.ObjectModel;

namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Nhan_vien
    {
        public Nhan_vien() {
            Hop_Dongs = new HashSet<Hop_dong>();
        }
        public string? MaNhanVien { get; set; }
        public string? Ten {  get; set; }
        public string? ChucVu {  get; set; }
        public string? Sdt { get; set; }
        public bool GioiTinh { get; set; }
        public string? Username { get; set; }
        public DateTime NgaySinh { get; set; }
        public virtual Toa? Toa { get; set; }
        public virtual ICollection<Hop_dong> Hop_Dongs { get; set; }
        public virtual User? User { get; set; }
    }
}
