using MailKit.Net.Smtp;
using MimeKit;

namespace IEEE_RegSys.Settings
{
    public class GmailEmailService
    {
        private readonly IConfiguration _config;

        public GmailEmailService(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendWithAttachmentAsync(
    string to,
    string subject,
    string htmlBody,
    string attachmentName,
    string contentType,
    byte[] attachmentBytes)
        {
            var settings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                settings["SenderName"],
                settings["SenderEmail"]
            ));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            builder.Attachments.Add(
                attachmentName,
                attachmentBytes,
                ContentType.Parse(contentType)
            );

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                settings["SmtpServer"],
                int.Parse(settings["SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                settings["SenderEmail"],
                settings["Password"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]
            ));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                emailSettings["SenderEmail"],
                emailSettings["Password"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }



    }
}
