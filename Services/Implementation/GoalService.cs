using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Services.Implementation
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddProgressAsync(int goalId, AddProgressViewModel model, string userId)
        {
            var goal = await GetGoalByIdAsync(goalId, userId);
            if (goal == null)
            {
                return false;
            }

            var progress = new GoalProgress
            {
                GoalId = goalId,
                Value = model.Value,
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.GoalProgresses.Add(progress);
            goal.CurrentValue += model.Value;

            if (goal.CurrentValue >= goal.TargetValue && goal.CompletedAt == null)
            {
                goal.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Goal> CreateGoalAsync(CreateGoalViewModel model, string userId)
        {
            var goal = new Goal
            {
                Title = model.Title,
                Description = model.Description,
                TargetValue = model.TargetValue,
                CurrentValue = model.CurrentValue,
                Unit = model.Unit,
                TargetDate = model.TargetDate,
                Category = model.Category,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<bool> DeleteGoalAsync(int id, string userId)
        {
            var goal = await GetGoalByIdAsync(id, userId);
            if (goal == null)
            {
                return false;
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProgressAsync(int progressId, string userId)
        {
            var progress = await _context.GoalProgresses
                .Include(p => p.Goal)
                .FirstOrDefaultAsync(p => p.Id == progressId && p.Goal.UserId == userId);

            if (progress == null)
            {
                return false;
            }

            progress.Goal.CurrentValue -= progress.Value;

            if (progress.Goal.CurrentValue < 0)
            {
                progress.Goal.CurrentValue = 0;
            }

            if (progress.Goal.CurrentValue < progress.Goal.TargetValue)
            {
                progress.Goal.CompletedAt = null; // Reset completion if current value drops below target
            }

            _context.GoalProgresses.Remove(progress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GoalSummaryViewModel>> GetActiveGoalsSummaryAsync(string userId)
        {
            var goals = await _context.Goals
                .Where(g => g.UserId == userId && g.CompletedAt == null)
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
                .ToListAsync();

            return goals.Select(g => new GoalSummaryViewModel
            {
                Id = g.Id,
                Title = g.Title,
                TargetValue = g.TargetValue,
                CurrentValue = g.CurrentValue,
                Unit = g.Unit,
                ProgressPercentage = g.ProgressPercentage
            });
        }

        public async Task<Goal?> GetGoalByIdAsync(int id, string userId)
        {
            return await _context.Goals
                .Include(g => g.ProgressEntries.OrderByDescending(p => p.CreatedAt))
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        }

        public async Task<IEnumerable<GoalProgress>> GetGoalProgressAsync(int goalId, string userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

            if (goal == null)
            {
                return new List<GoalProgress>();
            }

            return await _context.GoalProgresses
                .Where(p => p.GoalId == goalId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<GoalStatistics> GetGoalStatisticsAsync(int goalId, string userId)
        {
            var goal = await GetGoalByIdAsync(goalId, userId);
            if (goal == null)
            {
                return new GoalStatistics();
            }

            var progressEntries = goal.ProgressEntries.OrderBy(p => p.CreatedAt).ToList();

            if (!progressEntries.Any())
            {
                return new GoalStatistics
                {
                    TotalProgressEntries = 0,
                    DaysSinceLastProgress = (DateTime.UtcNow - goal.CreatedAt).Days
                };
            }

            var daysSinceCreation = Math.Max(1, (DateTime.UtcNow - goal.CreatedAt).Days);
            var averagePerDay = goal.CurrentValue / daysSinceCreation;

            var lastProgress = progressEntries.Last();
            var daysSinceLastProgress = (DateTime.UtcNow - lastProgress.CreatedAt).Days;

            DateTime? projectedDate = null;
            if (averagePerDay > 0 && !goal.IsCompleted)
            {
                var remainingValue = goal.TargetValue - goal.CurrentValue;
                var daysToComplete = (int)Math.Ceiling((double)(remainingValue / averagePerDay));
                projectedDate = DateTime.Today.AddDays(daysToComplete);
            }

            return new GoalStatistics
            {
                AverageProgressPerDay = Math.Round(averagePerDay, 2),
                ProjectedCompletionValue = goal.CurrentValue + (averagePerDay * (goal.TargetDate?.Subtract(DateTime.Today).Days ?? 30)),
                ProjectedCompletionDate = projectedDate,
                TotalProgressEntries = progressEntries.Count,
                LargestSingleProgress = progressEntries.Max(p => p.Value),
                LastProgressDate = lastProgress.CreatedAt,
                DaysSinceLastProgress = daysSinceLastProgress
            };
        }

        public async Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .Include(g => g.ProgressEntries)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateGoalAsync(int id, EditGoalViewModel model, string userId)
        {
            var goal = await GetGoalByIdAsync(id, userId);
            if (goal == null)
            {
                return false;
            }

            goal.Title = model.Title;
            goal.Description = model.Description;
            goal.TargetValue = model.TargetValue;
            goal.Unit = model.Unit;
            goal.TargetDate = model.TargetDate;
            goal.Category = model.Category;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
