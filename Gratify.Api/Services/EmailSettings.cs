namespace Gratify.Api.Services
{
    public class EmailSettings
    {
        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}