using FluentAssertions;
using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Implementation;
using HabitGoalTrackerApp.Services.Interfaces;
using HabitGoalTrackerApp.Tests.Unit.Helpers;
using Moq;

namespace HabitGoalTrackerApp.Tests.Unit.Services;

public class HabitServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly HabitService _habitService;
    private readonly Mock<IGoalService> _mockGoalService;
    private readonly string _userId = "test-user-id";

    public HabitServiceTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _mockGoalService = new Mock<IGoalService>();
        _habitService = new HabitService(_context, _mockGoalService.Object);
        
        // Seed test user
        var user = TestDataBuilder.CreateUser(_userId);
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateHabitAsync_ShouldCreateHabit()
    {
        // Arrange
        var viewModel = new CreateHabitViewModel
        {
            Title = "Morning Exercise",
            Description = "Daily morning routine",
            RepeatType = RepeatType.Daily
        };

        // Act
        var result = await _habitService.CreateHabitAsync(viewModel, _userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Morning Exercise");
        result.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task DeleteHabitAsync_ShouldRemoveHabit()
    {
        // Arrange
        var viewModel = new CreateHabitViewModel { Title = "Test Habit", RepeatType = RepeatType.Daily };
        var habit = await _habitService.CreateHabitAsync(viewModel, _userId);

        // Act
        var result = await _habitService.DeleteHabitAsync(habit.Id, _userId);

        // Assert
        result.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
