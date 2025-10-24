using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.DTOs
{
    public class CreateHabitDto
    {
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format")]
        public required string ColorHex { get; set; }
        public required int Frequency { get; set; }
    }
}
