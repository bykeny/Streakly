using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models.ViewModels
{
    public enum GoalCategory
    {
        Personal = 0,
        Health = 1,
        Career = 2,
        Education = 3,
        Finance = 4,
        Fitness = 5,
        Hobby = 6,
        Travel = 7,
        Relationship = 8,
        Other = 9
    }

    public class CreateGoalViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Goal title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Goal description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target value must be greater than zero.")]
        [Display(Name = "Target Value")]
        public decimal TargetValue { get; set; }

        [Display(Name = "Current Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Current value cannot be negative.")]
        public decimal CurrentValue { get; set; } = 0;

        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        [Display(Name = "Unit (e.g., books, pounds, hours)")]
        public string? Unit { get; set; }

        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [Display(Name = "Category")]
        public GoalCategory Category { get; set; } = GoalCategory.Personal;
    }

    public class EditGoalViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Goal title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Goal description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target value must be greater than zero.")]
        [Display(Name = "Target Value")]
        public decimal TargetValue { get; set; }

        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        [Display(Name = "Unit (e.g., books, pounds, hours)")]
        public string? Unit { get; set; }

        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [Display(Name = "Category")]
        public GoalCategory Category { get; set; } = GoalCategory.Personal;
    }

    public class GoalListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string? Unit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public GoalCategory Category { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOverDue { get; set; }
        public int DaysRemaining { get; set; }
        public string CategoryDisplay { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
    }

    public class GoalDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string? Unit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public GoalCategory Category { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOverDue { get; set; }
        public int DaysRemaining { get; set; }
        public string CategoryDisplay { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public List<GoalProgressViewModel> ProgressEntries { get; set; } = new();
        public GoalStatistics Statistics { get; set; } = new();
    }

    public class AddProgressViewModel
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Progress value must be greater than zero.")]
        [Display(Name = "Progress Value")]
        public decimal Value { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }

    public class GoalProgressViewModel
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal RunningTotal { get; set; }
    }

    public class GoalStatistics
    {
        public decimal AverageProgressPerDay { get; set; }
        public decimal ProjectedCompletionValue { get; set; }
        public DateTime? ProjectedCompletionDate { get; set; }
        public int TotalProgressEntries { get; set; }
        public decimal LargestSingleProgress { get; set; }
        public DateTime? LastProgressDate { get; set; }
        public int DaysSinceLastProgress { get; set; }
    }
}
