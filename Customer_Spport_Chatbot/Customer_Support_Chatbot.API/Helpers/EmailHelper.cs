using System.Net.Mail;

namespace Customer_Support_Chatbot.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendMail(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = _configuration["Smtp:Host"]!,
                    Port = int.Parse(_configuration["Smtp:Port"]!),
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(
                        _configuration["Smtp:Username"],
                        _configuration["Smtp:Password"])
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:FromEmail"]!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}