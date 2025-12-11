using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;

namespace IEEE_RegSys.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;
        public EmailHelper(IConfiguration config) { _config = config; }


        public async Task SendEmailAsync(string to, string subject, string body, string[]? attachments = null)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("IEEE Event", _config["Smtp:User"]!));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (File.Exists(file)) builder.Attachments.Add(file);
                }
            }

            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(
                _config["Smtp:Host"]!,
                int.Parse(_config["Smtp:Port"] ?? "587"),
                MailKit.Security.SecureSocketOptions.StartTls
            );
            await client.AuthenticateAsync(
                _config["Smtp:User"]!,
                _config["Smtp:Pass"]!
            );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
