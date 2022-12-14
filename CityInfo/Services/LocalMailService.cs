namespace CityInfo.Services
{
    public class LocalMailService : IMailService
    {
        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailTo"];
            _mailFrom = configuration["mailSettings:mailFrom"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo} with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
