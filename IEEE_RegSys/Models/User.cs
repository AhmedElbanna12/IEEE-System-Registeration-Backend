using System.ComponentModel.DataAnnotations;

namespace IEEE_RegSys.Models
{
    public class User 
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!; // For demo store plain - replace with proper hash
        public string Role { get; set; } = "Attendee"; // Admin | Attendee
    }
}
