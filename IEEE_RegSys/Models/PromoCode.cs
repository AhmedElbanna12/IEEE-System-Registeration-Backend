using System.ComponentModel.DataAnnotations;

namespace IEEE_RegSys.Models
{
    public class PromoCode
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = null!; // EX: IEEE2026

        [Required]
        public int DiscountPercentage { get; set; } // 0 - 100

        public bool IsActive { get; set; } = true;

        public int UsageLimit { get; set; } = 1;

        public int UsedCount { get; set; } = 0;

        public DateTime? ExpiryDate { get; set; }
    }
}
