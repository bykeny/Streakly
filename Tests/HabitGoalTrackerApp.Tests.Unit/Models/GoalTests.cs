using FluentAssertions;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Tests.Unit.Models;

public class GoalTests
{
    [Fact]
    public void IsCompleted_ShouldReturnTrueWhenCurrentValueReachesTarget()
    {
        // Arrange
        var goal = new Goal
        {
            TargetValue = 100,
            CurrentValue = 100
        };

        // Act & Assert
        goal.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void IsCompleted_ShouldReturnFalseWhenBelowTarget()
    {
        // Arrange
        var goal = new Goal
        {
            TargetValue = 100,
            CurrentValue = 50
        };

        // Act & Assert
        goal.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void ProgressPercentage_ShouldCalculateCorrectly()
    {
        // Arrange
        var goal = new Goal
        {
            TargetValue = 100,
            CurrentValue = 25
        };

        // Act
        var percentage = goal.ProgressPercentage;

        // Assert
        percentage.Should().Be(25);
    }

    [Fact]
    public void IsOverdue_ShouldReturnTrueWhenTargetDatePassedAndNotCompleted()
    {
        // Arrange
        var goal = new Goal
        {
            TargetDate = DateTime.UtcNow.AddDays(-1),
            TargetValue = 100,
            CurrentValue = 50
        };

        // Act & Assert
        goal.IsOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_ShouldReturnFalseWhenCompleted()
    {
        // Arrange
        var goal = new Goal
        {
            TargetDate = DateTime.UtcNow.AddDays(-1),
            TargetValue = 100,
            CurrentValue = 100,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        // Act & Assert
        goal.IsOverdue.Should().BeFalse();
    }

    [Fact]
    public void DaysRemaining_ShouldCalculateCorrectly()
    {
        // Arrange
        var goal = new Goal
        {
            TargetDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var days = goal.DaysRemaining;

        // Assert
        days.Should().BeInRange(9, 10); // Allow small variance due to timing
    }

    [Fact]
    public void GetCategoryDisplay_ShouldReturnFriendlyName()
    {
        // Arrange & Act & Assert
        new Goal { Category = GoalCategory.Health }.GetCategoryDisplay()
            .Should().Be("Health");
        
        new Goal { Category = GoalCategory.Fitness }.GetCategoryDisplay()
            .Should().Be("Fitness");
    }
}
