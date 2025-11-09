using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Services.Interfaces
{
    public interface IInsightsService
    {
        Task<WeeklyInsightsViewModel?> GetWeeklyInsightsAsync(string userId);
    }
}

