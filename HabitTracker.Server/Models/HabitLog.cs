namespace HabitTracker.Server.Models
{
    public class HabitLog
    {
        public Guid Id { get; set; }
        public Guid HabitId { get; set; }
        public DateTime CompletedDate { get; set; }
        public string? Notes { get; set; }
        public Habit Habit { get; set; }
    }
}
