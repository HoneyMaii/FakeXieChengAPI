using System;
using System.ComponentModel.DataAnnotations;

namespace FakeXieCheng.API.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Emial { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
