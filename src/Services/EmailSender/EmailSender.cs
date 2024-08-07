using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Services
{
    public class EmailSender : IEmailSender
    {
        private Email _email { get; }

        public EmailSender(IOptions<Email> optionsAccessor)
        {
            _email = optionsAccessor.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            MimeMessage mineMessage = new MimeMessage();
            mineMessage.From.Add(MailboxAddress.Parse(_email.Username));
            mineMessage.To.Add(MailboxAddress.Parse(toEmail));
            mineMessage.Subject = subject;

            mineMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_email.Host, 587, SecureSocketOptions.StartTls);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_email.Username, _email.Password);

                await client.SendAsync(mineMessage);
                client.Disconnect(true);
            }
        }
    }
}