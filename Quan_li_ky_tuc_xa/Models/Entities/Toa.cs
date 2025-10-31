namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Toa
    {
        public Toa() { 
            Phongs = new HashSet<Phong>();
        }
        public string? MaToa { get; set; }
        public string? Ten {  get; set; }
        public string? MaNhanVienQuanLi { get; set; }
        public virtual Nhan_vien? Nhan_Vien { get; set; }
        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
