namespace HabitGoalTrackerApp.Models.ViewModels
{
    public class WeeklyInsightsViewModel
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyInsights { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public string? Encouragement { get; set; }
        public bool HasData { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}

