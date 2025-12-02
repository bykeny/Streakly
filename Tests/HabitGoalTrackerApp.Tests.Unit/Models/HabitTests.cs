using FluentAssertions;
using HabitGoalTrackerApp.Models;

namespace HabitGoalTrackerApp.Tests.Unit.Models;

public class HabitTests
{
    [Fact]
    public void IsScheduledForDate_Daily_ShouldReturnTrueForAnyDay()
    {
        // Arrange
        var habit = new Habit { RepeatType = RepeatType.Daily };

        // Act & Assert
        habit.IsScheduledForDate(DateTime.Now).Should().BeTrue();
        habit.IsScheduledForDate(DateTime.Now.AddDays(5)).Should().BeTrue();
    }

    [Fact]
    public void IsScheduledForDate_Weekdays_ShouldReturnTrueForWeekdaysOnly()
    {
        // Arrange
        var habit = new Habit { RepeatType = RepeatType.Weekdays };

        // Act & Assert
        var monday = new DateTime(2025, 12, 1); // Monday
        var saturday = new DateTime(2025, 12, 6); // Saturday
        
        habit.IsScheduledForDate(monday).Should().BeTrue();
        habit.IsScheduledForDate(saturday).Should().BeFalse();
    }

    [Fact]
    public void IsScheduledForDate_Weekends_ShouldReturnTrueForWeekendsOnly()
    {
        // Arrange
        var habit = new Habit { RepeatType = RepeatType.Weekends };

        // Act & Assert
        var monday = new DateTime(2025, 12, 1); // Monday
        var saturday = new DateTime(2025, 12, 6); // Saturday
        
        habit.IsScheduledForDate(monday).Should().BeFalse();
        habit.IsScheduledForDate(saturday).Should().BeTrue();
    }

    [Fact]
    public void GetRepeatScheduleDisplay_ShouldReturnCorrectString()
    {
        // Arrange & Act & Assert
        new Habit { RepeatType = RepeatType.Daily }.GetRepeatScheduleDisplay()
            .Should().Be("Every day");
        
        new Habit { RepeatType = RepeatType.Weekdays }.GetRepeatScheduleDisplay()
            .Should().Be("Weekdays (Mon-Fri)");
        
        new Habit { RepeatType = RepeatType.Weekends }.GetRepeatScheduleDisplay()
            .Should().Be("Weekends (Sat-Sun)");
    }
}
