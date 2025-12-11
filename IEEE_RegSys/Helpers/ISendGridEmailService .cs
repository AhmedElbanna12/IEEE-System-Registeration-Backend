namespace IEEE_RegSys.Helpers;

public interface ISendGridEmailService
{
    Task SendAsync(string to, string subject, string body);
    Task SendWithAttachmentAsync(string to, string subject, string body, string fileName, string contentType, byte[] fileBytes);
}

