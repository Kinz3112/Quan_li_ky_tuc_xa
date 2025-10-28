using System.Collections.ObjectModel;

namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Sinh_Vien
    {
        public Sinh_Vien() { 
            Hop_Dongs = new HashSet<Hop_dong>();
        }
        public string MaSinhVien { get; set; }
        public string HovaTen { get; set; }
        public string Lop {  get; set; }
        public string Khoa { get; set; }
        public string Sdt { get; set; }
        public bool GioiTinh { get; set; }//true la nam, false la nu
        public DateTime NgaySinh {  get; set; }
        public string MaPhong { get; set; }
        public virtual Phong? Phong { get; set; }
        public virtual ICollection<Hop_dong> Hop_Dongs { get; set; }
        public virtual Phong? TruongPhong { get; set; }
    }
}
