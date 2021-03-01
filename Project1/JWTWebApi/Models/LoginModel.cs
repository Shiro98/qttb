using System.ComponentModel.DataAnnotations;

namespace JWTWebApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Mời nhập tên đăng nhập.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mời nhập mật khẩu.")]
        public string Password { get; set; }
        public string RememberMe { get; set; }
        //Code link app
        public string AppCode { get; set; } 
    }
}