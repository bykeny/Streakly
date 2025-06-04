using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Services.Implementation
{
    public class HabitService : IHabitService
    {
        private readonly ApplicationDbContext _context;
        public HabitService(ApplicationDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<Habit> CreateHabitAsync(CreateHabitViewModel model, string userId)
        {
            var habit = new Habit
            {
                Title = model.Title,
                Description = model.Description,
                IconName = model.IconName,
                Color = model.Color,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return habit;
        }

        public async Task<bool> DeleteHabitAsync(int id, string userId)
        {
            var habit = await GetHabitByIdAsync(id, userId);
            if (habit == null)
            {
                return false;
            }

            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var habits = await GetUserHabitsAsync(userId);
            var habitsList = new List<HabitListViewModel>();

            foreach (var habit in habits.Where(h => h.IsActive))
            {
                var isCompletedToday = await IsHabitCompletedTodayAsync(habit.Id);
                var streak = await GetHabitStreakAsync(habit.Id);

                var last7days = new List<bool>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    var completed = habit.Completions
                        .Any(c=> c.CompletedDate.Date == date);
                    last7days.Add(completed);
                }

                habitsList.Add(new HabitListViewModel
                {
                    Id = habit.Id,
                    Title = habit.Title,
                    Description = habit.Description,
                    IconName = habit.IconName,
                    Color = habit.Color,
                    IsActive = habit.IsActive,
                    CreatedAt = habit.CreatedAt,
                    CurrentStreak = streak,
                    IsCompletedToday = isCompletedToday,
                    Last7Days = last7days
                });
            }

            var completedTodayCount = habitsList.Count(h => h.IsCompletedToday);
            var longestStreak = habitsList.Any() ? habitsList.Max(h => h.CurrentStreak) : 0;

            var totalPossibleCompletions = habitsList.Count * 7; // 7 days a week
            var actualCompletions = habitsList.Sum(h => h.Last7Days.Count(d => d));
            var weeklyCompletionRate = totalPossibleCompletions > 0 ? (double)actualCompletions / totalPossibleCompletions * 100 : 0;

            return new DashboardViewModel
            {
                UserName = userId,
                TotalHabits = habitsList.Count,
                CompletedTodayCount = completedTodayCount,
                CurrentLongestStreak = longestStreak,
                WeeklyCompletionRate = Math.Round(weeklyCompletionRate, 1),
                TodaysHabits = habitsList.Take(5).ToList(), // Limit to 5 habits for dashboard
                ActiveGoals = new List<GoalSummaryViewModel>()
            };
        }

        public async Task<Habit?> GetHabitByIdAsync(int id, string userId)
        {
            return await _context.Habits
                .Include(h => h.Completions)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        }

        public async Task<IEnumerable<HabitCompletion>> GetHabitCompletionsAsync(int habitId, DateTime startDate, DateTime endDate)
        {
            return await _context.HabitCompletions
                .Where(hc => hc.HabitId == habitId && 
                            hc.CompletedDate.Date >= startDate.Date && 
                            hc.CompletedDate.Date <= endDate.Date)
                .OrderBy(hc => hc.CompletedDate).
                ToListAsync();
        }

        public async Task<int> GetHabitStreakAsync(int habitId)
        {
            var completions = await _context.HabitCompletions
                .Where(hc => hc.HabitId == habitId)
                .Select(hc => hc.CompletedDate.Date)
                .OrderByDescending(d => d)
                .ToListAsync();

            if (completions.Count == 0) return 0;

            var streak = 0;
            var currentDate = DateTime.Today;

            // check if completed today or yesterday
            if (completions.Contains(currentDate))
            {
                streak = 1;
                currentDate = currentDate.AddDays(-1);
            }
            else if (completions.Contains(currentDate.AddDays(-1)))
            {
                streak = 1;
                currentDate = currentDate.AddDays(-2);
            }
            else
            {
                return 0; // not completed today or yesterday
            }

            while (completions.Contains(currentDate))
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }

            return streak;
        }

        public async Task<IEnumerable<Habit>> GetUserHabitsAsync(string userId)
        {
            return await _context.Habits
                .Where(h => h.UserId == userId)
                .Include(h => h.Completions)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> MarkHabitCompleteAsync(int habitId, string userId, DateTime? date = null)
        {
            var habit = await GetHabitByIdAsync(habitId, userId);
            if (habit == null)
            {
                return false;
            }

            var completionDate = date ?? DateTime.UtcNow.Date;

            var existingCompletion = await _context.HabitCompletions
                .FirstOrDefaultAsync(hc => hc.HabitId == habitId && hc.CompletedDate.Date == completionDate);

            if (existingCompletion != null) return true; // Already marked as complete

            var completion = new HabitCompletion
            {
                HabitId = habitId,
                CompletedDate = completionDate,
                CreatedAt = DateTime.UtcNow,
            };

            _context.HabitCompletions.Add(completion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnmarkHabitCompleteAsync(int habitId, string userId, DateTime? date = null)
        {
            var habit = await GetHabitByIdAsync(habitId, userId);
            if (habit == null)
            {
                return await Task.FromResult(false);
            }

            var completionDate = date?.Date ?? DateTime.Today;

            var completion = await _context.HabitCompletions
                .FirstOrDefaultAsync(hc => hc.HabitId == habitId && hc.CompletedDate.Date == completionDate);

            if (completion == null) return true; // Not marked as complete

            _context.HabitCompletions.Remove(completion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHabitAsync(int id, EditHabitViewModel model, string userId)
        {
            var habit = await GetHabitByIdAsync(id, userId);
            if (habit == null)
            {
                return false;
            }

            habit.Title = model.Title;
            habit.Description = model.Description;
            habit.IconName = model.IconName;
            habit.Color = model.Color;
            habit.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsHabitCompletedTodayAsync(int habitId)
        {
            return await _context.HabitCompletions
                .AnyAsync(hc => hc.HabitId == habitId && hc.CompletedDate.Date == DateTime.Today);
        }
    }
}
