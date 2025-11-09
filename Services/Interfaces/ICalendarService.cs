using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Services.Interfaces
{
    public interface ICalendarService
    {
        Task<CalendarViewModel> GetCalendarDataAsync(string userId, int year, int month);
        Task<List<CalendarHeatmapData>> GetHeatmapDataAsync(string userId, DateTime startDate, DateTime endDate);
        Task<CalendarStatsViewModel> GetCalendarStatsAsync(string userId, int year, int month);
    }
}
