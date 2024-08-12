using System.ComponentModel.DataAnnotations;

namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class Login
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}