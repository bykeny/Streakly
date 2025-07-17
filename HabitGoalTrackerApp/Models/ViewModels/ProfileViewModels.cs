using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }

        [Display(Name = "Date Format")]
        public string DateFormat { get; set; } = "MM/dd/yyyy";

        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; } = true;

        [Display(Name = "Daily Reminders")]
        public bool DailyReminders { get; set; } = true;

        [Display(Name = "Weekly Reports")]
        public bool WeeklyReports { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }

        // Statistics
        public int TotalHabits { get; set; }
        public int TotalGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int CurrentStreak { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
