using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HabitGoalTrackerApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHabitService _habitService;
        private readonly IGoalService _goalService;
        private readonly IInsightsService _insightsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IHabitService habitService, IGoalService goalService, IInsightsService insightsService, UserManager<ApplicationUser> userManager)
        {
            _habitService = habitService;
            _goalService = goalService;
            _insightsService = insightsService;
            _userManager = userManager;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            var dashboardData = await _habitService.GetDashboardDataAsync(userId);
            var activeGoals = await _goalService.GetActiveGoalsSummaryAsync(userId);

            dashboardData.ActiveGoals = activeGoals.ToList();

            // Get ML.NET-powered insights (non-blocking)
            try
            {
                dashboardData.WeeklyInsights = await _insightsService.GetWeeklyInsightsAsync(userId);
            }
            catch
            {
                dashboardData.WeeklyInsights = null;
            }

            return View(dashboardData);
        }
    }
}
