using HabitTracker.Server.Data;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Server.Services
{
    public class HabitService : IHabitService
    {
        private readonly ApplicationDbContext _context;

        public HabitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HabitResponseDto> CreateHabitAsync(string userId, CreateHabitDto dto)
        {
            Habit newHabit = new Habit()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Frequency = dto.Frequency,
                ColorHex = dto.ColorHex
            };

            _context.Habits.Add(newHabit);
            await _context.SaveChangesAsync();

            HabitResponseDto responseDto = new HabitResponseDto()
            {
                Id = newHabit.Id,
                Name = newHabit.Name,
                Description = newHabit.Description,
                Frequency = newHabit.Frequency,
                ColorHex = newHabit.ColorHex,
                Created = newHabit.Created,
            };

            return responseDto;
        }

        public async Task<bool> DeleteHabitAsync(string userId, Guid id)
        {
            Habit? habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.UserId == userId && h.Id == id);

            if (habit == null)
            {
                return false;
            }
            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HabitResponseDto>> GetAllHabitsAsync(string userId)
        {
            return await _context.Habits
            .Where(h => h.UserId == userId)
            .Select(habit => new HabitResponseDto()
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                Frequency = habit.Frequency,
                ColorHex = habit.ColorHex,
                Created = habit.Created,
            })
            .ToListAsync();
        }

        public async Task<HabitResponseDto?> GetHabitAsync(string userId, Guid id)
        {
            Habit? habit = await _context.Habits
                 .FirstOrDefaultAsync(h => h.UserId == userId && h.Id == id);

            if (habit == null)
            {
                return null;
            }

            return new HabitResponseDto
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                Frequency = habit.Frequency,
                ColorHex = habit.ColorHex,
                Created = habit.Created
            };
        }

        public async Task<HabitResponseDto?> UpdateHabitAsync(string userId, CreateHabitDto dto, Guid id)
        {
            Habit? existingHabit = await _context.Habits
                 .FirstOrDefaultAsync(h => h.UserId == userId && h.Id == id);

            if (existingHabit == null)
            {
                return null;
            }

            existingHabit.Frequency = dto.Frequency;
            existingHabit.Name = dto.Name;
            existingHabit.Description = dto.Description;
            existingHabit.ColorHex = dto.ColorHex;

            _context.Habits.Update(existingHabit);
            await _context.SaveChangesAsync();

            return new HabitResponseDto
            {
                Id = existingHabit.Id,
                Name = existingHabit.Name,
                Description = existingHabit.Description,
                Frequency = existingHabit.Frequency,
                ColorHex = existingHabit.ColorHex,
                Created = existingHabit.Created
            };
        }
    }
}
