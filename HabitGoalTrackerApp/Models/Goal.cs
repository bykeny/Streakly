using System.ComponentModel.DataAnnotations;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Models
{
    public class Goal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; } = 0;
        public string? Unit { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public GoalCategory Category { get; set; } = GoalCategory.Personal;
        
        public bool IsCompleted => CurrentValue >= TargetValue;
        public decimal ProgressPercentage => TargetValue > 0 ? Math.Min((CurrentValue / TargetValue) * 100, 100) : 0;

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<GoalProgress> ProgressEntries { get; set; } = new List<GoalProgress>();

        public bool IsOverdue => TargetDate.HasValue && TargetDate.Value.Date < DateTime.Today && !IsCompleted;
        public int DaysRemaining => TargetDate.HasValue ? Math.Max(0, (TargetDate.Value.Date - DateTime.Today).Days) : 0;

        public string GetCategoryDisplay()
        {
            return Category switch
            {
                GoalCategory.Personal => "Personal",
                GoalCategory.Health => "Health",
                GoalCategory.Career => "Career",
                GoalCategory.Education => "Education",
                GoalCategory.Finance => "Finance",
                GoalCategory.Fitness => "Fitness",
                GoalCategory.Hobby => "Hobby",
                GoalCategory.Travel => "Travel",
                GoalCategory.Relationship => "Relationship",
                GoalCategory.Other => "Other",
                _ => "Personal"
            };
        }

        public string GetStatusDisplay()
        {
            if (IsCompleted) return "Completed";
            if (IsOverdue) return "Overdue";
            if (TargetDate.HasValue && DaysRemaining <= 7) return "Due Soon";
            return "In Progress";
        }
    }
}
