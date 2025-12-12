using System.ComponentModel.DataAnnotations;

namespace IEEE_RegSys.Dtos
{
    public class RegisterDto
    {
        
        public string FullNameArabic { get; set; } = null!;
        public string FullNameEnglish { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string NationalID { get; set; } = null!;

        public string College { get; set; } = null!;

        public string AcademicYear { get; set; } = null!;

        public string Email { get; set; } = null!;
        public int Age { get; set; }

        public string Gender { get; set; } = null!;
       
        public string? PaymentCode { get; set; } = null!;

        public IFormFile? PaymentImage { get; set; }

        public bool IsNeedBus { get; set; }

    }
}
