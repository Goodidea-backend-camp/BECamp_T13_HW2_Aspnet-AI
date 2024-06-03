using System.ComponentModel.DataAnnotations;

namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class Register
    {
        public int id { get; set; }
        [Required]
        public string email { get; set; }
        [Key]
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}