using IEEE_RegSys.Context;
using IEEE_RegSys.Dtos;
using IEEE_RegSys.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IEEE_RegSys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckinController : ControllerBase
    {
        private readonly RegContext _db;
        private readonly QRHelper _qr;


        public CheckinController(RegContext db, QRHelper qr)
        {
            _db = db; _qr = qr;
        }


        [HttpPost]
        public async Task<IActionResult> Checkin([FromBody] CheckinRequest req)
        {
            // decode simple qr format: we used raw text starting with "attendee:"
            if (string.IsNullOrEmpty(req.QrData)) return BadRequest("qrData required");
            var txt = req.QrData;
            // parse attendee id
            var idToken = txt.Split(';').FirstOrDefault(p => p.StartsWith("attendee:"));
            if (idToken == null) return BadRequest("Invalid QR Code");
            if (!int.TryParse(idToken.Split(':')[1], out var attId)) return BadRequest("Invalid QR Code");


            var att = await _db.Attendees.FindAsync(attId);
            if (att == null) return NotFound("Attendee not found");
            if (att.Status != "Approved") return BadRequest("Attendee is not approved");


            att.CheckInTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();


            return Ok(new { message = "Checked in", attendee = att.Id, checkInTime = att.CheckInTime });
        }
    }
}
