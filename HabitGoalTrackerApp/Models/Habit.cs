using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models
{
    public class Habit
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<HabitCompletion> Completions { get; set; } = new List<HabitCompletion>();

    }
}
