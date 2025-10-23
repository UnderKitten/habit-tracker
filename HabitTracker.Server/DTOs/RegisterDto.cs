using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.DTOs
{
    public class RegisterDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
