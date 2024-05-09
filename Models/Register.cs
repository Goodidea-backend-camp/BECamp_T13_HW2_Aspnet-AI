using System.ComponentModel.DataAnnotations;

namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class Register : Login   // Inheritance email and username from class "Login".
    {
        public int id { get; set; }
        [Required]
        public string email { get; set; }
    }
}