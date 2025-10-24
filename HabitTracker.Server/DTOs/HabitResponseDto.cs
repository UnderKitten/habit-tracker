using HabitTracker.Server.Models;

namespace HabitTracker.Server.DTOs
{
    public class HabitResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Frequency { get; set; }
        public DateTime Created { get; set; } 
        public string ColorHex { get; set; }
    }
}
