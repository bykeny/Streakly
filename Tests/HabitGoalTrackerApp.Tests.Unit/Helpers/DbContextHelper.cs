using HabitGoalTrackerApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Tests.Unit.Helpers;

public static class DbContextHelper
{
    public static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
