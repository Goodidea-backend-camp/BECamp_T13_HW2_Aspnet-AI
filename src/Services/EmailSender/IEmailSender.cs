namespace BECamp_T13_HW2_Aspnet_AI.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}