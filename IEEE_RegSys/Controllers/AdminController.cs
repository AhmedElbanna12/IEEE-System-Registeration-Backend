using IEEE_RegSys.Context;
using IEEE_RegSys.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace IEEE_RegSys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly RegContext _db;
        private readonly QRHelper _qr;
        private readonly EmailHelper _emails;
        private readonly IWebHostEnvironment _env;

        private readonly ISendGridEmailService _email;

        public AdminController(RegContext db, QRHelper qr, ISendGridEmailService email, IWebHostEnvironment env , EmailHelper emails)
        {
            _db = db;
            _qr = qr;
            _email = email;
            _env = env;
            _emails = emails;
        }

        // GET /api/admin/attendees
        [HttpGet("attendees")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var q = _db.Attendees.AsQueryable();
            if (!string.IsNullOrEmpty(status)) q = q.Where(a => a.Status == status);
            var list = await q.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return Ok(list);
        }



        [HttpPost("attendees/{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            // 1️⃣ جلب المشارك
            var att = await _db.Attendees.FindAsync(id);
            if (att == null) return NotFound();

            // 2️⃣ تغيير الحالة
            att.Status = "Approved";

            // 3️⃣ توليد QR
            var qrBytes = _qr.GenerateQrBytes($"attendee:{att.Id};nid:{att.NationalID};name:{att.FullNameEnglish}");

            // 4️⃣ حفظ QR في wwwroot/qrcodes
            var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "qrcodes");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(folder, fileName);

            await System.IO.File.WriteAllBytesAsync(filePath, qrBytes);

            // حفظ المسار النسبي في قاعدة البيانات
            att.QRCodePath = Path.Combine("qrcodes", fileName);

            await _db.SaveChangesAsync();

            // 5️⃣ قراءة HTML Template
            var templatePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "email-templates", "email-template.html");
            if (!System.IO.File.Exists(templatePath)) return StatusCode(500, "Email template not found.");

            var html = await System.IO.File.ReadAllTextAsync(templatePath);

            // 6️⃣ دمج البيانات والمتغيرات
            var qrBase64 = Convert.ToBase64String(qrBytes);
            var qrBase64Image = $"data:image/png;base64,{qrBase64}";

            html = html.Replace("{{FULL_NAME}}", att.FullNameEnglish)
                       .Replace("{{EVENT_NAME}}", "IEEE Event 2025")
                       .Replace("{{QR_BASE64}}", qrBase64Image)
                       .Replace("{{NID}}", att.NationalID)
                       .Replace("{{EMAIL}}", att.Email)
                       .Replace("{{PHONE}}", att.Phone)
                       .Replace("{{DATE}}", DateTime.UtcNow.ToString("yyyy-MM-dd"));

            // 7️⃣ إرسال الإيميل
            try
            {
                // ممكن تختار دمج الـ QR في الـ HTML أو كمرفق
                await _email.SendWithAttachmentAsync(
     att.Email,
     "Your Registration is Approved!",
     html,
     "QRCode.png",
     "image/png",
     qrBytes
 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
                return StatusCode(500, "Error sending approval email.");
            }

            return Ok(new { message = "Approved & email sent." });
        }




        // POST /api/admin/attendees/{id}/cancel
        [HttpPost("attendees/{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var att = await _db.Attendees.FindAsync(id);
            if (att == null) return NotFound();

            att.Status = "Canceled";
            await _db.SaveChangesAsync();

            try
            {
                await _emails.SendEmailAsync(
                    att.Email,
                    "Registration Canceled",
                    $"<p>Dear {att.FullNameEnglish},</p><p>Your registration has been canceled.</p>"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }

            return Ok(new { message = "Canceled" });
        }
    }
}
