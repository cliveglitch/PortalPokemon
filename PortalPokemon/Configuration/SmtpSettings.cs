using MailKit.Security;

namespace PortalPokemon.Configuration
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseAuthentication { get; set; }
        public SecureSocketOptions Security { get; set; } = SecureSocketOptions.StartTls;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
