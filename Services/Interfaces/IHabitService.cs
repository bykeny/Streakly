using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Services.Interfaces
{
    public interface IHabitService
    {
        Task<IEnumerable<Habit>> GetUserHabitsAsync(string userId);
        Task<Habit?> GetHabitByIdAsync(int id, string userId);
        Task<Habit> CreateHabitAsync(CreateHabitViewModel model, string userId);
        Task<bool> UpdateHabitAsync(int id, EditHabitViewModel model, string userId);
        Task<bool> DeleteHabitAsync(int id, string userId);
        Task<bool> MarkHabitCompleteAsync(int habitId, string userId, DateTime? date = null);
        Task<bool> UnmarkHabitCompleteAsync(int habitId, string userId, DateTime? date = null);
        Task<int> GetHabitStreakAsync(int habitId);
        Task<bool> IsHabitCompletedTodayAsync(int habitId);
        Task<IEnumerable<HabitCompletion>> GetHabitCompletionsAsync(int habitId, DateTime startDate, DateTime endDate);
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
    }
}
