namespace FileUploadService.Models
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string FromPassword { get; set; }
    }
}
