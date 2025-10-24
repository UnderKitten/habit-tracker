using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitService
    {
        Task<HabitResponseDto> CreateHabitAsync(string userId, CreateHabitDto dto);
        Task<HabitResponseDto> UpdateHabitAsync(string userId, CreateHabitDto dto, Guid id);
        Task<bool> DeleteHabitAsync(string userId, Guid id);
        Task<HabitResponseDto?> GetHabitAsync(string userId, Guid id);
        Task<List<HabitResponseDto>> GetAllHabitsAsync(string userId);
    }
}
