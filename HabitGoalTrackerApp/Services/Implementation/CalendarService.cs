using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HabitGoalTrackerApp.Services.Implementation
{
    public class CalendarService : ICalendarService
    {
        private readonly ApplicationDbContext _context;

        public CalendarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CalendarViewModel> GetCalendarDataAsync(string userId, int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            var lastDayOfCalendar = firstDayOfCalendar.AddDays(41); // 6 weeks

            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .Include(h => h.Completions.Where(c => c.CompletedDate >= firstDayOfCalendar && c.CompletedDate <= lastDayOfCalendar))
                .ToListAsync();

            // Get goal progress for the period
            var goalProgress = await _context.GoalProgresses
                .Include(gp => gp.Goal)
                .Where(gp => gp.Goal.UserId == userId &&
                            gp.CreatedAt >= firstDayOfCalendar &&
                            gp.CreatedAt <= lastDayOfCalendar)
                .ToListAsync();

            // Build calendar weeks
            var weeks = new List<CalendarWeek>();
            var currentDate = firstDayOfCalendar;

            for (int week = 0; week < 6; week++)
            {
                var calendarWeek = new CalendarWeek();

                for (int day = 0; day < 7; day++)
                {
                    var calendarDay = BuildCalendarDay(currentDate, firstDayOfMonth, lastDayOfMonth, habits, goalProgress);
                    calendarWeek.Days.Add(calendarDay);
                    currentDate = currentDate.AddDays(1);
                }

                weeks.Add(calendarWeek);
            }

            // Build habit calendar data
            var habitCalendarData = habits.Select(h => new HabitCalendarData
            {
                Id = h.Id,
                Title = h.Title,
                Color = h.Color ?? "#3b82f6",
                IconName = h.IconName,
                IsActive = h.IsActive,
                CompletionData = h.Completions.ToDictionary(c => c.CompletedDate.Date, c => true),
                ScheduleData = BuildScheduleData(h, firstDayOfCalendar, lastDayOfCalendar)
            }).ToList();

            // Get stats
            var stats = await GetCalendarStatsAsync(userId, year, month);

            return new CalendarViewModel
            {
                Year = year,
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Weeks = weeks,
                Habits = habitCalendarData,
                Stats = stats,
                FirstDayOfMonth = firstDayOfMonth,
                LastDayOfMonth = lastDayOfMonth
            };
        }

        public async Task<CalendarStatsViewModel> GetCalendarStatsAsync(string userId, int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var habits = await _context.Habits
                .Where(h => h.UserId == userId && h.IsActive)
                .Include(h => h.Completions.Where(c => c.CompletedDate >= firstDayOfMonth && c.CompletedDate <= lastDayOfMonth))
                .ToListAsync();

            var goalProgress = await _context.GoalProgresses
                .Include(gp => gp.Goal)
                .Where(gp => gp.Goal.UserId == userId &&
                            gp.CreatedAt >= firstDayOfMonth &&
                            gp.CreatedAt <= lastDayOfMonth)
                .ToListAsync();

            // Calculate daily completion rates
            var dailyRates = new List<DailyCompletionRate>();
            var currentDate = firstDayOfMonth;

            while (currentDate <= lastDayOfMonth)
            {
                var scheduledHabits = habits.Where(h => h.IsScheduledForDate(currentDate)).ToList();
                var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == currentDate)).Count();
                var rate = scheduledHabits.Count > 0 ? (decimal)completedHabits / scheduledHabits.Count * 100 : 0;

                dailyRates.Add(new DailyCompletionRate
                {
                    Date = currentDate,
                    Rate = rate
                });

                currentDate = currentDate.AddDays(1);
            }

            // Calculate streaks
            var currentStreak = CalculateCurrentStreak(habits);
            var longestStreak = CalculateLongestStreak(habits, firstDayOfMonth, lastDayOfMonth);

            return new CalendarStatsViewModel
            {
                TotalActiveHabits = habits.Count,
                AverageCompletionRate = (int)Math.Round(dailyRates.Average(d => d.Rate)),
                BestDay = dailyRates.Any() ? (int)dailyRates.Max(d => d.Rate) : 0,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                DailyRates = dailyRates,
                TotalGoalProgress = goalProgress.Count,
                GoalProgressValue = goalProgress.Sum(gp => gp.Value)
            };
        }

        public async Task<List<CalendarHeatmapData>> GetHeatmapDataAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var habits = await _context.Habits
                .Where(h => h.UserId == userId && h.IsActive)
                .Include(h => h.Completions.Where(c => c.CompletedDate >= startDate && c.CompletedDate <= endDate))
                .ToListAsync();

            var heatmapData = new List<CalendarHeatmapData>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                var scheduledHabits = habits.Where(h => h.IsScheduledForDate(currentDate)).ToList();
                var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == currentDate)).Count();
                var totalHabits = scheduledHabits.Count;

                var completionRate = totalHabits > 0 ? (decimal)completedHabits / totalHabits * 100 : 0;
                var level = CalculateHeatmapLevel(completionRate);

                heatmapData.Add(new CalendarHeatmapData
                {
                    Date = currentDate,
                    CompletedHabits = completedHabits,
                    TotalHabits = totalHabits,
                    CompletionRate = completionRate,
                    Level = level
                });

                currentDate = currentDate.AddDays(1);
            }

            return heatmapData;
        }

        private CalendarDay BuildCalendarDay(DateTime date, DateTime firstDayOfMonth, DateTime lastDayOfMonth,
            List<Habit> habits, List<GoalProgress> goalProgress)
        {
            var isCurrentMonth = date >= firstDayOfMonth && date <= lastDayOfMonth;
            var scheduledHabits = habits.Where(h => h.IsActive && h.IsScheduledForDate(date)).ToList();
            var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == date.Date)).ToList();

            var habitStatuses = scheduledHabits.Select(h => new HabitCompletionStatus
            {
                HabitId = h.Id,
                HabitTitle = h.Title,
                Color = h.Color ?? "#3b82f6",
                IsCompleted = h.Completions.Any(c => c.CompletedDate.Date == date.Date),
                IsScheduled = true
            }).ToList();

            var dayGoalProgress = goalProgress
                .Where(gp => gp.CreatedAt.Date == date.Date)
                .Select(gp => new GoalProgressEntry
                {
                    GoalId = gp.GoalId,
                    GoalTitle = gp.Goal.Title,
                    ProgressValue = gp.Value,
                    Unit = gp.Goal.Unit,
                    Notes = gp.Notes
                }).ToList();

            var completionPercentage = scheduledHabits.Count > 0
                ? (decimal)completedHabits.Count / scheduledHabits.Count * 100
                : 0;

            return new CalendarDay
            {
                Date = date,
                IsCurrentMonth = isCurrentMonth,
                IsToday = date.Date == DateTime.Today,
                HasHabits = scheduledHabits.Any(),
                CompletedHabits = completedHabits.Count,
                TotalScheduledHabits = scheduledHabits.Count,
                CompletionPercentage = completionPercentage,
                HabitStatuses = habitStatuses,
                GoalProgress = dayGoalProgress
            };
        }

        private Dictionary<DateTime, bool> BuildScheduleData(Habit habit, DateTime startDate, DateTime endDate)
        {
            var scheduleData = new Dictionary<DateTime, bool>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                scheduleData[currentDate] = habit.IsScheduledForDate(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            return scheduleData;
        }

        private int CalculateHeatmapLevel(decimal completionRate)
        {
            return completionRate switch
            {
                0 => 0,
                > 0 and <= 25 => 1,
                > 25 and <= 50 => 2,
                > 50 and <= 75 => 3,
                > 75 => 4,
                _ => 0
            };
        }

        private int CalculateCurrentStreak(List<Habit> habits)
        {
            if (!habits.Any()) return 0;

            var currentDate = DateTime.Today;
            var streak = 0;

            while (true)
            {
                var scheduledHabits = habits.Where(h => h.IsScheduledForDate(currentDate)).ToList();
                if (!scheduledHabits.Any())
                {
                    currentDate = currentDate.AddDays(-1);
                    continue;
                }

                var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == currentDate)).Count();
                var completionRate = (decimal)completedHabits / scheduledHabits.Count;

                if (completionRate >= 0.8m) // 80% completion rate required for streak
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        private int CalculateLongestStreak(List<Habit> habits, DateTime startDate, DateTime endDate)
        {
            if (!habits.Any()) return 0;

            var longestStreak = 0;
            var currentStreak = 0;
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var scheduledHabits = habits.Where(h => h.IsScheduledForDate(currentDate)).ToList();
                if (scheduledHabits.Any())
                {
                    var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == currentDate)).Count();
                    var completionRate = (decimal)completedHabits / scheduledHabits.Count;

                    if (completionRate >= 0.8m)
                    {
                        currentStreak++;
                        longestStreak = Math.Max(longestStreak, currentStreak);
                    }
                    else
                    {
                        currentStreak = 0;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            return longestStreak;
        }
    }
}
