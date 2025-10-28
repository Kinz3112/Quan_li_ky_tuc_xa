namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Phong
    {
        public Phong() { 
            Sinh_Viens = new HashSet<Sinh_Vien>();
            Hop_Dongs = new HashSet<Hop_dong>();
        }
        public string MaPhong { get; set; }
        public string Ten { get; set; }
        public string MaToa { get; set; }
        public string LoaiPhong { get;set; }
        public int SoNguoi { get;set; }//so ng o hien tai
        public string MaTruongPhong { get; set; }
        public virtual Sinh_Vien Truong_phong { get; set; }
        public virtual Toa Toa { get; set; }
        public virtual ICollection<Sinh_Vien> Sinh_Viens { get; set; }
        public virtual ICollection<Hop_dong> Hop_Dongs { get; set; }
    }
}
