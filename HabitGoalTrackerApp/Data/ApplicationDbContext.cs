using HabitGoalTrackerApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitCompletion> HabitCompletions { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalProgress> GoalProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Habit>()
                .HasOne(h => h.User)
                .WithMany(u => u.Habits)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HabitCompletion>()
                .HasOne(hc => hc.Habit)
                .WithMany(h => h.Completions)
                .HasForeignKey(hc => hc.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Goal>()
                .HasOne(g => g.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GoalProgress>()
                .HasOne(gp => gp.Goal)
                .WithMany(g => g.ProgressEntries)
                .HasForeignKey(gp => gp.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Goal>()
                .Property(g => g.TargetValue)
                .HasPrecision(18, 2);

            builder.Entity<Goal>()
                .Property(g => g.CurrentValue)
                .HasPrecision(18, 2);

            builder.Entity<GoalProgress>()
                .Property(gp => gp.Value)
                .HasPrecision(18, 2);

            builder.Entity<HabitCompletion>()
                .HasIndex(hc => new { hc.HabitId, hc.CompletedDate })
                .IsUnique();
        }
    }
}
