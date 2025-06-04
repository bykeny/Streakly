using Microsoft.AspNetCore.Identity;

namespace HabitGoalTrackerApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    }
}
