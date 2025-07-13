using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Components.Web;

namespace HabitGoalTrackerApp.Services.Interfaces
{
    public interface IGoalService
    {
        Task<IEnumerable<Goal>> GetUserGoalsAsync(string userId);
        Task<Goal?> GetGoalByIdAsync(int id, string userId);
        Task<Goal> CreateGoalAsync(CreateGoalViewModel model, string userId);
        Task<bool> UpdateGoalAsync(int id, EditGoalViewModel model, string userId);
        Task<bool> DeleteGoalAsync(int id, string userId);
        Task<bool> AddProgressAsync(int goalId, AddProgressViewModel model, string userId);
        Task<bool> DeleteProgressAsync(int progressId, string userId);
        Task<IEnumerable<GoalProgress>> GetGoalProgressAsync(int goalId, string userId);
        Task<GoalStatistics> GetGoalStatisticsAsync(int goalId, string userId);
        Task<IEnumerable<GoalSummaryViewModel>> GetActiveGoalsSummaryAsync(string userId);
    }
}
