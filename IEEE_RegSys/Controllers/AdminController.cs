using IEEE_RegSys.Context;
using IEEE_RegSys.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace IEEE_RegSys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly RegContext _db;
        private readonly QRHelper _qr;
        private readonly EmailHelper _email;
        private readonly IWebHostEnvironment _env;


        public AdminController(RegContext db, QRHelper qr, EmailHelper email, IWebHostEnvironment env)
        {
            _db = db; _qr = qr; _email = email; _env = env;
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


        // POST /api/admin/attendees/{id}/approve
        [HttpPost("attendees/{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var att = await _db.Attendees.FindAsync(id);
            if (att == null) return NotFound();


            att.Status = "Approved";


            // generate QR
            var qrBytes = _qr.GenerateQrBytes($"attendee:{att.Id};nid:{att.NationalID};name:{att.FullNameEnglish}");
            var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "qrcodes");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var filename = Guid.NewGuid() + ".png";
            var path = Path.Combine(folder, filename);
            await System.IO.File.WriteAllBytesAsync(path, qrBytes);
            att.QRCodePath = Path.Combine("qrcodes", filename);


            await _db.SaveChangesAsync();


            // send email with QR
            await _email.SendEmailAsync(att.Email, "Registration Approved", "Your registration is approved. Attached is your QR.", new[] { path });


            return Ok(new { message = "Approved" });
        }


        // POST /api/admin/attendees/{id}/cancel
        [HttpPost("attendees/{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var att = await _db.Attendees.FindAsync(id);
            if (att == null) return NotFound();
            att.Status = "Canceled";
            await _db.SaveChangesAsync();
            await _email.SendEmailAsync(att.Email, "Registration Canceled", "Your registration has been canceled.");
            return Ok(new { message = "Canceled" });
        }
    }
}

