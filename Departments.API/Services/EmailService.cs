
using System.Net.Mail;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Departments.API._Models.DTO;
using System.Text.Json;

namespace Departments.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        public EmailService(ILogger<EmailService> logger)
        {
            this.logger = logger;
        }
        public async Task SendEmailAsync(ICollection<string> RecipientsEmail, string subject, string body)
        {
            try
            {
                string _smtpUser = "tester13624@gmail.com";
                string _smtpPassword = "qbwo amlv prtj dlwj ";

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUser),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };

                    foreach (var recipient in RecipientsEmail)
                    {
                        mailMessage.To.Add(recipient.Trim());
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                    logger.LogInformation($" Finished sent new  Reminder to :{JsonSerializer.Serialize(string.Join(",", RecipientsEmail))}");
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, $"{ex.Message}");
            }
         
        }
    }
}
