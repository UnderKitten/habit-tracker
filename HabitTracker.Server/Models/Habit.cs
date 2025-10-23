using Microsoft.AspNetCore.Identity;

namespace HabitTracker.Server.Models
{
    public class Habit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int Frequency { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string ColorHex { get; set; }
        public List<HabitLog> HabitLogs { get; set; }
    }
}
