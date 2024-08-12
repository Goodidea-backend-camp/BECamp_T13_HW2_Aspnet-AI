using System.ComponentModel.DataAnnotations;

namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class Register
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ComfirmPassword { get; set; } = string.Empty;
    }
}