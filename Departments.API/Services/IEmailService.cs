using Departments.API._Models.Domain;

namespace Departments.API.Services
{
    public interface IEmailService
    {
         Task SendEmailAsync(ICollection<string> RecipientsEmail, string subject, string body);
    }
}
