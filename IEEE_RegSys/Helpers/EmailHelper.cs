using MailKit.Net.Smtp;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace IEEE_RegSys.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Send an email using SendGrid
        /// </summary>
        /// <param name="to">Recipient email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">HTML body of the email</param>
        /// <param name="attachments">Optional file paths to attach</param>
        public async Task SendEmailAsync(string to, string subject, string body, string[]? attachments = null)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("SendGrid API key is missing in configuration.");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("hr.benisuef.ieee@gmail.com", "IEEE Event");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent: null, htmlContent: body);

            // Attach files if any
            if (attachments != null)
            {
                foreach (var filePath in attachments)
                {
                    if (File.Exists(filePath))
                    {
                        var bytes = await File.ReadAllBytesAsync(filePath);
                        var fileBase64 = Convert.ToBase64String(bytes);
                        msg.AddAttachment(Path.GetFileName(filePath), fileBase64);
                    }
                }
            }

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var respBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid failed: {response.StatusCode}, {respBody}");
            }
        }
    }
}
