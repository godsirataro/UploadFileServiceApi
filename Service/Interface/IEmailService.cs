using FileUploadService.Models;

namespace FileUploadService.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string message);
    }
}
