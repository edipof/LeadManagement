
using LeadManagement.Application.Interfaces;

namespace LeadManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailNotificationAsync(string to, string subject, string message)
        {
            string emailContent = $"To: {to}\nSubject: {subject}\nMessage: {message}";
            await File.WriteAllTextAsync("email_notification.txt", emailContent);
        }
    }
}
