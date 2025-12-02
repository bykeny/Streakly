using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HabitGoalTrackerApp.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ICalendarService _calendarService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CalendarController(ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            _calendarService = calendarService;
            _userManager = userManager;
        }

        // GET: calendar
        [Route("calendar")]
        public async Task<IActionResult> Index(int? year, int? month)
        {
            var currentDate = DateTime.Today;
            var targetYear = year ?? currentDate.Year;
            var targetMonth = month ?? currentDate.Month;

            var userId = _userManager.GetUserId(User)!;
            var calendarData = await _calendarService.GetCalendarDataAsync(userId, targetYear, targetMonth);

            return View(calendarData);
        }

        // GET: calendar/heatmap
        [Route("calendar/heatmap")]
        public async Task<IActionResult> Heatmap(DateTime? startDate, DateTime? endDate)
        {
            var userId = _userManager.GetUserId(User)!;

            var start = startDate ?? DateTime.Today.AddDays(-365);
            var end = endDate ?? DateTime.Today;

            var heatmapData = await _calendarService.GetHeatmapDataAsync(userId, start, end);

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            return View(heatmapData);
        }

        // API endpoint for AJAX requests
        [Route("calendar/get-calendar-data")]
        [HttpGet]
        public async Task<IActionResult> GetCalendarData(int year, int month)
        {
            var userId = _userManager.GetUserId(User)!;
            var calendarData = await _calendarService.GetCalendarDataAsync(userId, year, month);

            return Json(calendarData);
        }

        // API endpoint for heatmap data
        [Route("calendar/get-heat-map-data")]
        [HttpGet]
        public async Task<IActionResult> GetHeatMapData(DateTime startDate, DateTime endDate)
        {
            var userId = _userManager.GetUserId(User)!;
            var heatmapData = await _calendarService.GetHeatmapDataAsync(userId, startDate, endDate);

            return Json(heatmapData);
        }
    }
}
