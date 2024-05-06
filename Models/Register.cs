namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class Register : Login   // Inheritance email and username from class "Login".
    {
        public string username { get; set; }
        public string about_me { get; set; }
    }
}