namespace FileUploadService.Models
{
    public class JwtSettings
    {
        public int JwtExpireMinutes { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public string JwtKey { get; set; }
    }
}