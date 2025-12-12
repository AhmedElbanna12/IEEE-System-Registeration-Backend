using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace IEEE_RegSys.Models
{
    public class Attendee
    {
        public int Id { get; set; }


        [Required]
        public string FullNameArabic { get; set; } = null!;
        [Required]
        public string FullNameEnglish { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
        [Required]

        public string Governorate { get; set; } = null!;
        [Required]

        public string NationalID { get; set; } = null!;
        [Required]

        public string College { get; set; } = null!;
        [Required]

        public string AcademicYear { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]

        public int Age { get; set; }
        [Required]

        public string Gender { get; set; } = null!;
        [Required]

        public string? PaymentImagePath { get; set; }
        public string? PaymentCode { get; set; }

        [Required]

        public string Status { get; set; } = "Pending"; // Pending | Approved | Canceled


        public string? QRCodePath { get; set; }

        public bool IsNeedBus { get; set; }

        public bool IsIEEEIAN { get; set; }
        public DateTime? CheckInTime { get; set; }

        [Required]

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
