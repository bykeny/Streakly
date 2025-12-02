using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Tests.Unit.Helpers;

public static class TestDataBuilder
{
    public static ApplicationUser CreateUser(string id = "test-user-id", string email = "test@example.com")
    {
        return new ApplicationUser
        {
            Id = id,
            UserName = email,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Habit CreateHabit(string userId, string title = "Test Habit", RepeatType repeatType = RepeatType.Daily)
    {
        return new Habit
        {
            Title = title,
            Description = "Test description",
            RepeatType = repeatType,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static Goal CreateGoal(string userId, string title = "Test Goal", decimal targetValue = 100)
    {
        return new Goal
        {
            Title = title,
            Description = "Test goal description",
            TargetValue = targetValue,
            CurrentValue = 0,
            Category = GoalCategory.Personal,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            TargetDate = DateTime.UtcNow.AddDays(30)
        };
    }

    public static JournalEntry CreateJournalEntry(string userId, string title = "Test Journal")
    {
        return new JournalEntry
        {
            Title = title,
            Content = "Test journal content",
            Mood = JournalMood.Happy,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static HabitCompletion CreateCompletion(int habitId, DateTime date)
    {
        return new HabitCompletion
        {
            HabitId = habitId,
            CompletedDate = date.Date,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static GoalProgress CreateProgress(int goalId, decimal value, string notes = "Test progress")
    {
        return new GoalProgress
        {
            GoalId = goalId,
            Value = value,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }
}
