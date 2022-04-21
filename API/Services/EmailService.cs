using API.Helpers;
using API.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        readonly EmailSettings _settings;
        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task<bool> SendEmail(EmailMessage message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Rent Shop", "admin"));
            emailMessage.To.Add(new MailboxAddress("", message.Email.Trim()));
            emailMessage.Subject = message.Title;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message.Message
            };

            using (var client = new SmtpClient())
            {

                System.Console.WriteLine(_settings.Email);
                System.Console.WriteLine(_settings.Password);

                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate(_settings.Email, _settings.Password);
                client.Send(emailMessage);

                await client.DisconnectAsync(true);
            }

            return true;
        }
    }
}