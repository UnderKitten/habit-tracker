namespace HabitTracker.Server.DTOs
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
}
