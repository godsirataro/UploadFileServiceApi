using FileUploadService.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MailKit.Security;

namespace FileUploadService.Service
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings, IOptions<EmailSettings> emailSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("html") { Text = message };

                foreach (var to in _emailSettings.To)
                {
                    if (!string.IsNullOrEmpty(to))
                    {
                        emailMessage.To.Add(new MailboxAddress("", to));
                    }
                }

                if (_emailSettings.Cc != null)
                {
                    foreach (var cc in _emailSettings.Cc)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            emailMessage.To.Add(new MailboxAddress("", cc));
                        }
                    }
                }

                if (_emailSettings.Bcc != null)
                {
                    foreach (var bcc in _emailSettings.Bcc)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            emailMessage.To.Add(new MailboxAddress("", bcc));
                        }
                    }
                }

                using (var client = new SmtpClient())
                {
                    // Set up client properties
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Disable certificate validation for now

                    // Connect with STARTTLS
                    await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);

                    // Authenticate and send email
                    await client.AuthenticateAsync(_smtpSettings.FromEmail, _smtpSettings.FromPassword);
                    await client.SendAsync(emailMessage);

                    // Disconnect
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here
                // You can log the exception, send a notification, etc.
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }
        }

    }
}
