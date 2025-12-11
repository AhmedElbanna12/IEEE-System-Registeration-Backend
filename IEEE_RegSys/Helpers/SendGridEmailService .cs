using IEEE_RegSys.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IEEE_RegSys.Helpers;

public class SendGridEmailService: ISendGridEmailService
{
    private readonly SendGridSettings _settings;

    public SendGridEmailService(IOptions<SendGridSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var toEmail = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

        var response = await client.SendEmailAsync(msg);
    }

    public async Task SendWithAttachmentAsync(
        string to, string subject, string body,
        string fileName, string contentType, byte[] fileBytes)
    {
        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var toEmail = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

        // إضافة الـ Attachment (الـ QR Code)
        var fileBase64 = Convert.ToBase64String(fileBytes);
        msg.AddAttachment(fileName, fileBase64, contentType);

        var response = await client.SendEmailAsync(msg);
    }
}
