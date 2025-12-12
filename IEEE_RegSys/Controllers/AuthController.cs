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
        private readonly ILogger<AdminController> _logger;


        public AuthController(RegContext db, IConfiguration config, EmailHelper emailHelper, IWebHostEnvironment env, ISendGridEmailService emailService , ILogger<AdminController> logger)
        {
            _db = db;
            _config = config;
            _email = emailHelper;
            _env = env;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            _logger.LogInformation("🔵 Register endpoint called at: {time}", DateTime.UtcNow);

            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("⚠ Invalid model state: {@model}", ModelState);
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("📥 Received DTO: {@dto}", dto);

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
                    IsNeedBus = dto.IsNeedBus,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                // Save Payment Image
                if (dto.PaymentImage != null)
                {
                    _logger.LogInformation("📸 Payment image received: Name={name}, Size={size}",
                        dto.PaymentImage.FileName, dto.PaymentImage.Length);

                    var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "payment");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                        _logger.LogInformation("📂 Created payment folder: {folder}", folder);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.PaymentImage.FileName);
                    var filePath = Path.Combine(folder, fileName);

                    _logger.LogInformation("💾 Saving payment image to: {path}", filePath);

                    using var stream = System.IO.File.Create(filePath);
                    await dto.PaymentImage.CopyToAsync(stream);

                    attendee.PaymentImagePath = Path.Combine("payment", fileName);
                }
                else
                {
                    _logger.LogWarning("⚠ No payment image uploaded!");
                }

                // DB Add
                _db.Attendees.Add(attendee);
                await _db.SaveChangesAsync();

                _logger.LogInformation("✅ Saved attendee with ID={id}", attendee.Id);

                // Send Email
                _logger.LogInformation("📧 Sending pending email to: {email}", attendee.Email);

                await _emailService.SendAsync(attendee.Email,
                    "Thanks for registering!",
                    "Hello!, We received your registration and it’s now under review." +
                    "We will contact you once it's approved." +
                    "Thank you!");

                _logger.LogInformation("📨 Email sent successfully to {email}", attendee.Email);

                return Ok(new { message = "Registered and under review." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "🔥 ERROR in Register endpoint: Message={msg}, Inner={inner}, Stack={stack}",
                    ex.Message,
                    ex.InnerException?.Message,
                    ex.StackTrace);

                return StatusCode(500, "An error occurred while processing your request.");
            }
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

