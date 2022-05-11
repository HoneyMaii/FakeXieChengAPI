using System.ComponentModel.DataAnnotations;

namespace FakeXieCheng.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required] 
        [Compare(nameof(Password),ErrorMessage = "密码输入不一致")]
        public string ConfirmPassword { get; set; }
    }
}