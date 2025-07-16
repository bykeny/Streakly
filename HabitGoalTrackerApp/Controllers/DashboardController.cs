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
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IHabitService habitService, IGoalService goalService, UserManager<ApplicationUser> userManager)
        {
            _habitService = habitService;
            _goalService = goalService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            var dashboardData = await _habitService.GetDashboardDataAsync(userId);
            var activeGoals = await _goalService.GetActiveGoalsSummaryAsync(userId);

            dashboardData.ActiveGoals = activeGoals.ToList();

            return View(dashboardData);
        }
    }
}
