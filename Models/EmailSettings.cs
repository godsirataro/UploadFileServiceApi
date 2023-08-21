namespace FileUploadService.Models
{
    public class EmailSettings
    {
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        //public string Subject { get; set; }
        //public string Message { get; set; }
    }
}