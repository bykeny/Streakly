namespace HabitGoalTrackerApp.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; } = 0;
        public string? Unit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted => CurrentValue >= TargetValue;
        public decimal ProgressPercentage => TargetValue > 0 ? (CurrentValue / TargetValue) * 100 : 0;
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<GoalProgress> ProgressEntries { get; set; } = new List<GoalProgress>();
    }
}
