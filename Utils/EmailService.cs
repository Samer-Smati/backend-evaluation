using MailKit.Net.Smtp;
using MimeKit;

namespace PfeProject.Utils
{
    public class EmailService
    {
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var senderEmail = config["EmailSettings:Email"];
            var senderPassword = config["EmailSettings:Password"];
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Your Name", "your-email@gmail.com"));
            email.To.Add(new MailboxAddress("", recipientEmail));
            email.Subject = subject;

            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, senderPassword);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}