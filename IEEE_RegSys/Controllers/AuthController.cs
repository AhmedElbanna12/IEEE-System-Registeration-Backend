using IEEE_RegSys.Context;
using IEEE_RegSys.Dtos;
using IEEE_RegSys.Helpers;
using IEEE_RegSys.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace IEEE_RegSys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RegContext _db;
        private readonly IConfiguration _config;
        private readonly EmailHelper _email;
        private readonly IWebHostEnvironment _env;
        private readonly ISendGridEmailService _emailService;


        public AuthController(RegContext db, IConfiguration config, EmailHelper emailHelper, IWebHostEnvironment env, ISendGridEmailService emailService)
        {
            _db = db;
            _config = config;
            _email = emailHelper;
            _env = env;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendee = new Attendee
            {
                FullNameArabic = dto.FullNameArabic,
                FullNameEnglish = dto.FullNameEnglish,
                Phone = dto.Phone,
                Governorate = dto.Governorate,
                NationalID = dto.NationalID,
                College = dto.College,
                AcademicYear = dto.AcademicYear,
                Email = dto.Email,
                Age = dto.Age,
                Gender = dto.Gender,
                PaymentCode = dto.PaymentCode,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // حفظ صورة الدفع إذا موجودة
            if (dto.PaymentImage != null)
            {
                var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "payment");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.PaymentImage.FileName);
                var filePath = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(filePath);
                await dto.PaymentImage.CopyToAsync(stream);

                attendee.PaymentImagePath = Path.Combine("payment", fileName);
            }

            _db.Attendees.Add(attendee);
            await _db.SaveChangesAsync();


            // إرسال إيميل إن الحساب قيد المراجعة
            await _emailService.SendAsync(attendee.Email,
                "Registration Received",
                "Your registration is under review.");

           

            return Ok(new { message = "Registered and under review." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, [FromServices] JwtHelper jwtHelper)
        {
            // Try users table first
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username && u.PasswordHash == req.Password);
            if (user != null)
            {
                var token = jwtHelper.GenerateToken(user.Id.ToString(), user.Role);
                return Ok(new { token, userId = user.Id, role = user.Role });
            }


            // Try attendees by email
            var att = await _db.Attendees.FirstOrDefaultAsync(a => a.Email == req.Username);
            if (att != null)
            {
                // For attendees we don't have password: allow login by NationalID as a simple demo
                if (req.Password == att.NationalID)
                {
                    var token = jwtHelper.GenerateToken(att.Id.ToString(), "Attendee");
                    return Ok(new { token, userId = att.Id, role = "Attendee" });
                }
            }


            return Unauthorized();
        }
}
}

