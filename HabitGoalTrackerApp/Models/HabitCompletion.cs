namespace HabitGoalTrackerApp.Models
{
    public class HabitCompletion
    {
        public int Id { get; set; }
        public DateTime CompletedDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; } = null!;
    }
}
