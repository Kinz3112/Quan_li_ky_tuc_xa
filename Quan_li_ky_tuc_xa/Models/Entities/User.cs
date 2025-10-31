using System.ComponentModel.DataAnnotations;

namespace Quan_li_ky_tuc_xa.Models.Entities
{
    public class User
    {
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public virtual Sinh_Vien? Sinh_Vien { get; set; }
        public virtual Nhan_vien? Nhan_Vien { get; set; }
    }
}
