using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.DTOs
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
