using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitGoalTrackerApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResetLastLoginAtValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reset all LastLoginAt values to NULL since existing values represent registration dates, not login dates
            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [LastLoginAt] = NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
