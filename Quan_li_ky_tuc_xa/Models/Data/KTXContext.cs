using Microsoft.EntityFrameworkCore;
using Quan_li_ky_tuc_xa.Models.Entities;

namespace Quan_li_ky_tuc_xa.Models.Data
{
    public class KTXContext : DbContext
    {
        public KTXContext(DbContextOptions<KTXContext> options) : base(options){}
        public DbSet<Sinh_Vien> Sinh_Viens { get; set; }
        public DbSet<Nhan_vien> Nhan_Viens { get; set; }
        public DbSet<Hoa_don> Hoa_Dons { get; set; }
        public DbSet<Hop_dong> Hop_Dongs { get; set; }
        public DbSet<Toa> Toas { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(e => e.Id);
                entity.HasOne(r => r.User)
                      .WithOne(u => u.Role)
                      .HasForeignKey<Role>(r => r.Username)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Sinh_Vien>(entity =>
            {
                entity.ToTable("SinhVien");
                entity.HasKey(e => e.MaSinhVien);

                entity.HasOne(e => e.Phong)
                      .WithMany(p => p.Sinh_Viens)
                      .HasForeignKey(e => e.MaPhong)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(hd => hd.User)
                      .WithOne(h => h.Sinh_Vien)
                      .HasForeignKey<Sinh_Vien>(hd => hd.Username)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Nhan_vien>(entity =>
            {
                entity.ToTable("NhanVien");
                entity.HasKey(e => e.MaNhanVien);

                entity.HasOne(hd => hd.User)
                      .WithOne(h => h.Nhan_Vien)
                      .HasForeignKey<Nhan_vien>(hd => hd.Username)
                      .OnDelete(DeleteBehavior.Restrict);

            });

           

            // --------------------- TÒA ---------------------
            modelBuilder.Entity<Toa>(entity =>
            {
                entity.ToTable("Toa");
                entity.HasKey(e => e.MaToa);

                entity.HasOne(t => t.Nhan_Vien)
                      .WithOne(n => n.Toa)
                      .HasForeignKey<Toa>(t => t.MaNhanVienQuanLi)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------------- PHÒNG ---------------------
            modelBuilder.Entity<Phong>(entity =>
            {
                entity.ToTable("Phong");
                entity.HasKey(e => e.MaPhong);

                entity.HasOne(p => p.Toa)
                      .WithMany(t => t.Phongs)
                      .HasForeignKey(p => p.MaToa)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Truong_phong)
                      .WithOne(s => s.TruongPhong)
                      .HasForeignKey<Phong>(p => p.MaTruongPhong)
                      .OnDelete(DeleteBehavior.Restrict);
            });

           
            
            // --------------------- HỢP ĐỒNG ---------------------
            modelBuilder.Entity<Hop_dong>(entity =>
            {
                entity.ToTable("HopDong");
                entity.HasKey(e => e.MaHopDong);

                entity.HasOne(h => h.Nhan_Vien)
                      .WithMany(n => n.Hop_Dongs)
                      .HasForeignKey(h => h.MaNhanVienQuanLi)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Sinh_Vien)
                      .WithMany(s => s.Hop_Dongs)
                      .HasForeignKey(h => h.MaSinhVien)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Phong)
                      .WithMany(p => p.Hop_Dongs)
                      .HasForeignKey(h => h.MaPhong)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------------- HÓA ĐƠN ---------------------
            modelBuilder.Entity<Hoa_don>(entity =>
            {
                entity.ToTable("HoaDon");
                entity.HasKey(e => e.MaHoaDon);

                entity.HasOne(hd => hd.Hop_Dong)
                      .WithOne(h => h.Hoa_Don)
                      .HasForeignKey<Hoa_don>(hd => hd.MaHopDong)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --------------------- USER ---------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.Username);
            });
        }
    }
}
