using System.Globalization;

namespace HabitGoalTrackerApp.Models.ViewModels
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public List<CalendarWeek> Weeks { get; set; } = new();
        public List<HabitCalendarData> Habits { get; set; } = new();
        public CalendarStatsViewModel Stats { get; set; } = new();
        public DateTime FirstDayOfMonth { get; set; }
        public DateTime LastDayOfMonth { get; set; }
    }

    public class CalendarWeek
    {
        public List<CalendarDay> Days { get; set; } = new();
    }

    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool HasHabits { get; set; }
        public int CompletedHabits { get; set; }
        public int TotalScheduledHabits { get; set; }
        public decimal CompletionPercentage { get; set; }
        public List<HabitCompletionStatus> HabitStatuses { get; set; } = new();
        public List<GoalProgressEntry> GoalProgress { get; set; } = new();
    }

    public class HabitCalendarData
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<DateTime, bool> CompletionData { get; set; } = new();
        public Dictionary<DateTime, bool> ScheduleData { get; set; } = new();
    }

    public class HabitCompletionStatus
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsScheduled { get; set; }
    }

    public class GoalProgressEntry
    {
        public int GoalId { get; set; }
        public string GoalTitle { get; set; } = string.Empty;
        public decimal ProgressValue { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }

    public class CalendarStatsViewModel
    {
        public int TotalActiveHabits { get; set; }
        public int AverageCompletionRate { get; set; }
        public int BestDay { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public List<DailyCompletionRate> DailyRates { get; set; } = new();
        public int TotalGoalProgress { get; set; }
        public decimal GoalProgressValue { get; set; }
    }

    public class DailyCompletionRate
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }

    public class CalendarHeatmapData
    {
        public DateTime Date { get; set; }
        public int CompletedHabits { get; set; }
        public int TotalHabits { get; set; }
        public decimal CompletionRate { get; set; }
        public int Level { get; set; } // 0-4 for heatmap intensity
    }
}
