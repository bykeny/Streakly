namespace HabitGoalTrackerApp.Models
{
    public class GoalProgress
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int GoalId { get; set; }
        public virtual Goal Goal { get; set; } = null!;
    }
}
