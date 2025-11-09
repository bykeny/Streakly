using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Services.Implementation
{
    public class InsightsService : IInsightsService
    {
        private readonly ApplicationDbContext _context;

        public InsightsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WeeklyInsightsViewModel?> GetWeeklyInsightsAsync(string userId)
        {
            try
            {
                // Get data from the past 7 days
                var endDate = DateTime.Today;
                var startDate = endDate.AddDays(-7);

                // Get habits with completions
                var habits = await _context.Habits
                    .Include(h => h.Completions)
                    .Where(h => h.UserId == userId && h.IsActive)
                    .ToListAsync();

                // Get goals
                var goals = await _context.Goals
                    .Include(g => g.ProgressEntries)
                    .Where(g => g.UserId == userId && g.CompletedAt == null)
                    .ToListAsync();

                // Get journal entries
                var journalEntries = await _context.JournalEntries
                    .Where(j => j.UserId == userId && j.CreatedAt >= startDate && j.CreatedAt <= endDate)
                    .OrderBy(j => j.CreatedAt)
                    .ToListAsync();

                // Check if we have enough data
                if (!habits.Any() && !goals.Any() && !journalEntries.Any())
                {
                    return new WeeklyInsightsViewModel
                    {
                        HasData = false,
                        Summary = "Start tracking your habits, goals, or journal entries to receive personalized insights!"
                    };
                }

                // Analyze patterns using ML.NET-style analysis
                var insights = AnalyzePatterns(habits, goals, journalEntries, startDate, endDate);

                return insights;
            }
            catch (Exception)
            {
                return new WeeklyInsightsViewModel
                {
                    HasData = false,
                    Summary = "Unable to generate insights at this time. Please try again later."
                };
            }
        }

        private WeeklyInsightsViewModel AnalyzePatterns(
            List<Habit> habits, 
            List<Goal> goals, 
            List<JournalEntry> journalEntries, 
            DateTime startDate, 
            DateTime endDate)
        {
            var insights = new List<string>();
            var recommendations = new List<string>();
            var summaryParts = new List<string>();

            // Analyze habit completion patterns
            if (habits.Any())
            {
                var habitAnalysis = AnalyzeHabits(habits, startDate, endDate);
                insights.AddRange(habitAnalysis.Insights);
                recommendations.AddRange(habitAnalysis.Recommendations);
                if (!string.IsNullOrEmpty(habitAnalysis.Summary))
                    summaryParts.Add(habitAnalysis.Summary);
            }

            // Analyze goal progress patterns
            if (goals.Any())
            {
                var goalAnalysis = AnalyzeGoals(goals, startDate, endDate);
                insights.AddRange(goalAnalysis.Insights);
                recommendations.AddRange(goalAnalysis.Recommendations);
                if (!string.IsNullOrEmpty(goalAnalysis.Summary))
                    summaryParts.Add(goalAnalysis.Summary);
            }

            // Analyze journal mood patterns
            if (journalEntries.Any())
            {
                var journalAnalysis = AnalyzeJournal(journalEntries);
                insights.AddRange(journalAnalysis.Insights);
                recommendations.AddRange(journalAnalysis.Recommendations);
                if (!string.IsNullOrEmpty(journalAnalysis.Summary))
                    summaryParts.Add(journalAnalysis.Summary);
            }

            // Cross-analysis: habits vs mood correlation
            if (habits.Any() && journalEntries.Any())
            {
                var correlation = AnalyzeHabitMoodCorrelation(habits, journalEntries, startDate, endDate);
                if (!string.IsNullOrEmpty(correlation))
                    insights.Add(correlation);
            }

            // Generate summary
            var summary = summaryParts.Any()
                ? string.Join(" ", summaryParts)
                : "You've been making progress this week! Keep up the great work.";

            // Generate encouragement
            var encouragement = GenerateEncouragement(habits, goals, journalEntries);

            return new WeeklyInsightsViewModel
            {
                Summary = summary,
                KeyInsights = insights.Take(5).ToList(),
                Recommendations = recommendations.Take(3).ToList(),
                Encouragement = encouragement,
                HasData = true,
                GeneratedAt = DateTime.UtcNow
            };
        }

        private (List<string> Insights, List<string> Recommendations, string Summary) AnalyzeHabits(
            List<Habit> habits, DateTime startDate, DateTime endDate)
        {
            var insights = new List<string>();
            var recommendations = new List<string>();
            var summaryParts = new List<string>();

            // Calculate completion rates
            var habitStats = habits.Select(h =>
            {
                var completions = h.Completions
                    .Where(c => c.CompletedDate.Date >= startDate && c.CompletedDate.Date <= endDate)
                    .ToList();
                var scheduledDays = Enumerable.Range(0, 7)
                    .Select(i => startDate.AddDays(i))
                    .Count(d => h.IsScheduledForDate(d));
                var completionRate = scheduledDays > 0 ? (double)completions.Count / scheduledDays * 100 : 0;
                var streak = CalculateStreak(h, endDate);

                return new
                {
                    Habit = h,
                    Completions = completions.Count,
                    ScheduledDays = scheduledDays,
                    CompletionRate = completionRate,
                    Streak = streak
                };
            }).ToList();

            // Find best performing habit
            var bestHabit = habitStats.OrderByDescending(h => h.CompletionRate).FirstOrDefault();
            if (bestHabit != null && bestHabit.CompletionRate >= 80)
            {
                insights.Add($"You're excelling at '{bestHabit.Habit.Title}' with a {bestHabit.CompletionRate:F0}% completion rate!");
            }

            // Find struggling habits
            var strugglingHabits = habitStats.Where(h => h.CompletionRate < 50 && h.ScheduledDays > 0).ToList();
            if (strugglingHabits.Any())
            {
                var habit = strugglingHabits.First();
                insights.Add($"'{habit.Habit.Title}' could use more attention - you completed it {habit.Completions} out of {habit.ScheduledDays} scheduled days.");
                recommendations.Add($"Try setting a reminder for '{habit.Habit.Title}' or break it into smaller steps.");
            }

            // Streak analysis
            var longestStreak = habitStats.Max(h => h.Streak);
            if (longestStreak >= 7)
            {
                var habitWithStreak = habitStats.First(h => h.Streak == longestStreak);
                insights.Add($"Amazing! You've maintained a {longestStreak}-day streak with '{habitWithStreak.Habit.Title}'!");
            }

            // Overall completion rate
            var totalCompletions = habitStats.Sum(h => h.Completions);
            var totalScheduled = habitStats.Sum(h => h.ScheduledDays);
            var overallRate = totalScheduled > 0 ? (double)totalCompletions / totalScheduled * 100 : 0;

            if (overallRate >= 80)
                summaryParts.Add($"You completed {overallRate:F0}% of your scheduled habits this week - excellent consistency!");
            else if (overallRate >= 60)
                summaryParts.Add($"You completed {overallRate:F0}% of your scheduled habits this week - good progress!");
            else if (overallRate > 0)
                summaryParts.Add($"You completed {overallRate:F0}% of your scheduled habits this week - there's room for improvement.");

            // Day of week pattern
            var dayPattern = AnalyzeDayOfWeekPattern(habits, startDate, endDate);
            if (!string.IsNullOrEmpty(dayPattern))
                insights.Add(dayPattern);

            return (insights, recommendations, string.Join(" ", summaryParts));
        }

        private (List<string> Insights, List<string> Recommendations, string Summary) AnalyzeGoals(
            List<Goal> goals, DateTime startDate, DateTime endDate)
        {
            var insights = new List<string>();
            var recommendations = new List<string>();
            var summaryParts = new List<string>();

            foreach (var goal in goals)
            {
                var progressThisWeek = goal.ProgressEntries
                    .Where(p => p.CreatedAt.Date >= startDate && p.CreatedAt.Date <= endDate)
                    .Sum(p => p.Value);

                var progressPercent = goal.ProgressPercentage;
                var daysRemaining = goal.DaysRemaining;

                if (progressThisWeek > 0)
                {
                    insights.Add($"Great progress on '{goal.Title}'! You added {progressThisWeek} {goal.Unit ?? "units"} this week.");
                }

                if (progressPercent >= 75 && !goal.IsCompleted)
                {
                    insights.Add($"You're almost there! '{goal.Title}' is {progressPercent:F0}% complete.");
                    recommendations.Add($"Push through the final stretch on '{goal.Title}' - you're so close!");
                }
                else if (progressPercent < 25 && goal.CreatedAt < startDate)
                {
                    insights.Add($"'{goal.Title}' is at {progressPercent:F0}% - consider breaking it into smaller milestones.");
                    recommendations.Add($"Set weekly mini-goals for '{goal.Title}' to maintain momentum.");
                }

                if (goal.IsOverdue)
                {
                    insights.Add($"⚠️ '{goal.Title}' is past its target date - consider adjusting the deadline.");
                    recommendations.Add($"Review and update the target date for '{goal.Title}' to keep it realistic.");
                }
                else if (daysRemaining > 0 && daysRemaining <= 7)
                {
                    insights.Add($"'{goal.Title}' is due in {daysRemaining} days - time to focus!");
                }
            }

            var completedGoals = goals.Count(g => g.IsCompleted);
            if (completedGoals > 0)
            {
                summaryParts.Add($"You've completed {completedGoals} goal(s) - fantastic achievement!");
            }

            return (insights, recommendations, string.Join(" ", summaryParts));
        }

        private (List<string> Insights, List<string> Recommendations, string Summary) AnalyzeJournal(
            List<JournalEntry> journalEntries)
        {
            var insights = new List<string>();
            var recommendations = new List<string>();
            var summaryParts = new List<string>();

            if (!journalEntries.Any()) return (insights, recommendations, "");

            // Mood analysis
            var moodCounts = journalEntries.GroupBy(j => j.Mood)
                .Select(g => new { Mood = g.Key, Count = g.Count(), Percentage = (double)g.Count() / journalEntries.Count * 100 })
                .OrderByDescending(m => m.Count)
                .ToList();

            var dominantMood = moodCounts.First();
            var moodName = dominantMood.Mood switch
            {
                JournalMood.VeryHappy => "very happy",
                JournalMood.Happy => "happy",
                JournalMood.Neutral => "neutral",
                JournalMood.Sad => "sad",
                JournalMood.VerySad => "very sad",
                _ => "neutral"
            };

            if (dominantMood.Percentage >= 60)
            {
                if (dominantMood.Mood >= JournalMood.Happy)
                {
                    insights.Add($"Your journal entries show you've been feeling {moodName} {dominantMood.Percentage:F0}% of the time - that's wonderful!");
                }
                else if (dominantMood.Mood <= JournalMood.Sad)
                {
                    insights.Add($"Your journal entries show you've been feeling {moodName} {dominantMood.Percentage:F0}% of the time.");
                    recommendations.Add("Consider focusing on self-care activities or reaching out for support.");
                }
            }

            // Consistency
            if (journalEntries.Count >= 5)
            {
                insights.Add($"You've been journaling consistently with {journalEntries.Count} entries this week - great reflection practice!");
            }
            else if (journalEntries.Count >= 3)
            {
                recommendations.Add("Try journaling daily to better track your thoughts and feelings.");
            }

            summaryParts.Add($"You wrote {journalEntries.Count} journal entries this week.");

            return (insights, recommendations, string.Join(" ", summaryParts));
        }

        private string AnalyzeHabitMoodCorrelation(
            List<Habit> habits, 
            List<JournalEntry> journalEntries, 
            DateTime startDate, 
            DateTime endDate)
        {
            // Simple correlation: days with more completed habits tend to have better moods
            var daysWithData = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i))
                .Where(d => journalEntries.Any(j => j.CreatedAt.Date == d))
                .Select(day =>
                {
                    var completions = habits.Sum(h => h.Completions.Count(c => c.CompletedDate.Date == day));
                    var entry = journalEntries.FirstOrDefault(j => j.CreatedAt.Date == day);
                    return new { Day = day, Completions = completions, Mood = entry?.Mood ?? JournalMood.Neutral };
                })
                .ToList();

            if (daysWithData.Count < 3) return "";

            var positiveCorrelation = daysWithData
                .Where(d => d.Completions >= habits.Count * 0.7 && d.Mood >= JournalMood.Happy)
                .Count();

            if (positiveCorrelation >= 2)
            {
                return "Interesting pattern: On days when you complete most of your habits, you tend to feel more positive!";
            }

            return "";
        }

        private string AnalyzeDayOfWeekPattern(List<Habit> habits, DateTime startDate, DateTime endDate)
        {
            var dayCompletions = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i))
                .GroupBy(d => d.DayOfWeek)
                .Select(g => new
                {
                    Day = g.Key,
                    Completions = habits.Sum(h => h.Completions.Count(c => 
                        c.CompletedDate.Date >= startDate && c.CompletedDate.Date <= endDate &&
                        c.CompletedDate.DayOfWeek == g.Key)),
                    Scheduled = habits.Sum(h => g.Count(d => h.IsScheduledForDate(d)))
                })
                .Where(d => d.Scheduled > 0)
                .OrderByDescending(d => (double)d.Completions / d.Scheduled)
                .ToList();

            if (dayCompletions.Count < 2) return "";

            var bestDay = dayCompletions.First();
            var worstDay = dayCompletions.Last();

            if (bestDay.Completions > 0 && worstDay.Completions < bestDay.Completions * 0.5)
            {
                return $"You're most consistent on {bestDay.Day}s and less so on {worstDay.Day}s - consider what makes {bestDay.Day}s work better!";
            }

            return "";
        }

        private string GenerateEncouragement(
            List<Habit> habits, 
            List<Goal> goals, 
            List<JournalEntry> journalEntries)
        {
            var totalCompletions = habits.Sum(h => h.Completions.Count);
            var activeGoals = goals.Count(g => !g.IsCompleted);
            var journalCount = journalEntries.Count;

            if (totalCompletions >= 20 && activeGoals > 0)
                return "You're building incredible momentum! Your consistency is paying off.";
            else if (totalCompletions >= 10)
                return "You're making steady progress. Every small step counts!";
            else if (journalCount >= 5)
                return "Your commitment to self-reflection is admirable. Keep it up!";
            else
                return "You're on the right track. Consistency is key to long-term success!";
        }

        private int CalculateStreak(Habit habit, DateTime endDate)
        {
            var completions = habit.Completions
                .Select(c => c.CompletedDate.Date)
                .OrderByDescending(d => d)
                .ToList();

            if (completions.Count == 0) return 0;

            var streak = 0;
            var currentDate = endDate;

            if (completions.Contains(currentDate) || completions.Contains(currentDate.AddDays(-1)))
            {
                while (completions.Contains(currentDate))
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
            }

            return streak;
        }
    }
}

