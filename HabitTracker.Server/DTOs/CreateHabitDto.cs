using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.DTOs
{
    public class CreateHabitDto
    {
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public required string ColorHex { get; set; }
        public required int Frequency { get; set; }
    }
}
