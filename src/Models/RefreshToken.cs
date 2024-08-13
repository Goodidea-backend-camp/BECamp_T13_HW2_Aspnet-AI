namespace BECamp_T13_HW2_Aspnet_AI.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}