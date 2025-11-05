namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? RoleName { get; set; }
        public virtual User? User { get; set; }
    }
}
