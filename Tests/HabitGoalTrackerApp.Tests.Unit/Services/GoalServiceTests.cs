using FluentAssertions;
using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Implementation;
using HabitGoalTrackerApp.Tests.Unit.Helpers;

namespace HabitGoalTrackerApp.Tests.Unit.Services;

public class GoalServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GoalService _goalService;
    private readonly string _userId = "test-user-id";

    public GoalServiceTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _goalService = new GoalService(_context);
        
        // Seed test user
        var user = TestDataBuilder.CreateUser(_userId);
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateGoalAsync_ShouldCreateGoal()
    {
        // Arrange
        var viewModel = new CreateGoalViewModel
        {
            Title = "Read 10 Books",
            Description = "Read 10 books this year",
            TargetValue = 10,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Category = GoalCategory.Personal
        };

        // Act
        var result = await _goalService.CreateGoalAsync(viewModel, _userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Read 10 Books");
        result.TargetValue.Should().Be(10);
        result.CurrentValue.Should().Be(0);
    }

    [Fact]
    public async Task DeleteGoalAsync_ShouldRemoveGoal()
    {
        // Arrange
        var viewModel = new CreateGoalViewModel
        {
            Title = "Test Goal",
            TargetValue = 100,
            TargetDate = DateTime.UtcNow.AddDays(30),
            Category = GoalCategory.Personal
        };
        var goal = await _goalService.CreateGoalAsync(viewModel, _userId);

        // Act
        var result = await _goalService.DeleteGoalAsync(goal.Id, _userId);

        // Assert
        result.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
