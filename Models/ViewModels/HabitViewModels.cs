using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models.ViewModels
{
    public class CreateHabitViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Habit name cannot exceed 100 characters.")]
        [Display(Name = "Habit Name")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Icon")]
        public string? IconName { get; set; }

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Repeat Schedule")]
        public RepeatType RepeatType { get; set; } = RepeatType.Daily;

        [Display(Name = "Custom Days")]
        public List<int> CustomDays { get; set; } = new();
    }

    public class EditHabitViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Habit name cannot exceed 100 characters.")]
        [Display(Name = "Habit Name")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Icon")]
        public string? IconName { get; set; }

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Repeat Schedule")]
        public RepeatType RepeatType { get; set; } = RepeatType.Daily;

        [Display(Name = "Custom Days")]
        public List<int> CustomDays { get; set; } = new();
    }

    public class HabitListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CurrentStreak { get; set; }
        public bool IsCompletedToday { get; set; }
        public List<bool> Last7Days { get; set; } = new();
        public string RepeatScheduleDisplay { get; set; } = string.Empty;
        public bool IsScheduledToday { get; set; } = true;
    }

    public class HabitDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public string? Color { get; set; }   
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalCompletions { get; set; }
        public bool IsCompletedToday { get; set; }
        public List<HabitCompletionDay> CompletionHistory { get; set; } = new();
        public string RepeatScheduleDisplay { get; set; } = string.Empty;
        public bool IsScheduledToday { get; set; } = true;
    }

    public class HabitCompletionDay
    {
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int TotalHabits { get; set; }
        public int CompletedTodayCount { get; set; }
        public int CurrentLongestStreak { get; set; }
        public double WeeklyCompletionRate { get; set; }
        public List<HabitListViewModel> TodaysHabits { get; set; } = new();
        public List<GoalSummaryViewModel> ActiveGoals { get; set; } = new();
        public WeeklyInsightsViewModel? WeeklyInsights { get; set; }
    }

    public class GoalSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string? Unit { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}