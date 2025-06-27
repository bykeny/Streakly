using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models
{
    public class Habit
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public string? Color { get; set; }
        public RepeatType RepeatType { get; set; } = RepeatType.Daily;
        public string? CustomDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<HabitCompletion> Completions { get; set; } = new List<HabitCompletion>();
        public bool IsScheduledForDate(DateTime date)
        {
            return RepeatType switch
            {
                RepeatType.Daily => true,
                RepeatType.Weekdays => date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday,
                RepeatType.Weekends => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
                RepeatType.Custom => IsCustomDayScheduled(date),
                _ => true
            };
        }
        private bool IsCustomDayScheduled(DateTime date)
        {
            if (string.IsNullOrEmpty(CustomDays)) return false;

            try
            {
                var days = System.Text.Json.JsonSerializer.Deserialize<int[]>(CustomDays);
                var dayOfWeek = (int)date.DayOfWeek; // 0 = Sunday, 1 = Monday, etc.
                return days?.Contains(dayOfWeek) == true;
            }
            catch
            {
                return false;
            }
        }
        public string GetRepeatScheduleDisplay()
        {
            return RepeatType switch
            {
                RepeatType.Daily => "Every day",
                RepeatType.Weekdays => "Weekdays (Mon-Fri)",
                RepeatType.Weekends => "Weekends (Sat-Sun)",
                RepeatType.Custom => GetCustomDaysDisplay(),
                _ => "Every day"
            };
        }
        private string GetCustomDaysDisplay()
        {
            if (string.IsNullOrEmpty(CustomDays)) return "Custom schedule";

            try
            {
                var days = System.Text.Json.JsonSerializer.Deserialize<int[]>(CustomDays);
                if (days == null || days.Length == 0) return "Custom schedule";

                var dayNames = days.Select(d => d switch
                {
                    0 => "Sun",
                    1 => "Mon",
                    2 => "Tue",
                    3 => "Wed",
                    4 => "Thu",
                    5 => "Fri",
                    6 => "Sat",
                    _ => ""
                }).Where(name => !string.IsNullOrEmpty(name));

                return string.Join(", ", dayNames);
            }
            catch
            {
                return "Custom schedule";
            }
        }
    }

    public enum RepeatType
    {
        Daily = 0,
        Weekdays = 1,
        Weekends = 2,
        Custom = 3
    }
}
