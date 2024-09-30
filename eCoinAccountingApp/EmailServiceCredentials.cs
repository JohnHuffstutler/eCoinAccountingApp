using Microsoft.Extensions.Options;

namespace eCoinAccountingApp
{
    public class EmailServiceCredentials
    {
        public string EmailSMTPUrl { get; private set; }
        public string PortNumber { get; private set; }
        public string EmailSMTPPasswordHash { get; private set; }
        public string EmailFromAddress { get; private set; }
        public string EmailFromName { get; private set; }
        public string EmailAppName { get; private set; }

        public EmailServiceCredentials(IOptions<EmailSettings> emailSettings)
        {
            EmailSMTPUrl = emailSettings.Value.EmailSMTPUrl;
            PortNumber = emailSettings.Value.PortNumber;
            EmailSMTPPasswordHash = emailSettings.Value.EmailSMTPPasswordHash;
            EmailFromAddress = emailSettings.Value.EmailFromAddress;
            EmailFromName = emailSettings.Value.EmailFromName;
            EmailAppName = emailSettings.Value.EmailAppName;
        }
    }

    // Model class for the email settings
    public class EmailSettings
    {
        public string EmailSMTPUrl { get; set; }
        public string PortNumber { get; set; }
        public string EmailSMTPPasswordHash { get; set; }
        public string EmailFromAddress { get; set; }
        public string EmailFromName { get; set; }
        public string EmailAppName { get; set; }
    }
}
