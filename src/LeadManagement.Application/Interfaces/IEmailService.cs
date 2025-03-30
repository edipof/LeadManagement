namespace LeadManagement.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailNotificationAsync(string to, string subject, string message);
    }
}
