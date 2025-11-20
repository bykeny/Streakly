using Microsoft.AspNetCore.Identity;

namespace HabitGoalTrackerApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? TimeZone { get; set; }
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public bool EmailNotifications { get; set; } = true;
        public bool DailyReminders { get; set; } = true;
        public bool WeeklyReports { get; set; } = true;
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
