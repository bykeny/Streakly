using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using Microsoft.AspNetCore.Authorization;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Controllers
{
    [Authorize]
    public class HabitsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IHabitService _habitService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HabitsController(IHabitService habitService, UserManager<ApplicationUser> userManager)
        {
            _habitService = habitService;
            _userManager = userManager;
        }

        // GET: habits
        [Route("habits")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var habits = await _habitService.GetUserHabitsAsync(userId);

            var viewModel = new List<HabitListViewModel>();

            foreach (var habit in habits)
            {
                var isCompletedToday = await _habitService.IsHabitCompletedTodayAsync(habit.Id);
                var streak = await _habitService.GetHabitStreakAsync(habit.Id);

                var last7Days = new List<bool>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    var completed = habit.Completions.Any(c => c.CompletedDate.Date == date);
                    last7Days.Add(completed);
                }

                viewModel.Add(new HabitListViewModel
                {
                    Id = habit.Id,
                    Title = habit.Title,
                    Description = habit.Description,
                    IconName = habit.IconName,
                    Color = habit.Color,
                    IsActive = habit.IsActive,
                    CreatedAt = habit.CreatedAt,
                    CurrentStreak = streak,
                    IsCompletedToday = isCompletedToday,
                    Last7Days = last7Days
                });
            }

            return View(viewModel);
        }

        // GET: habits/details/5
        [Route("habits/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var habit = await _habitService.GetHabitByIdAsync(id, userId);

            if (habit == null)
            {
                return NotFound();
            }

            var isCompletedToday = await _habitService.IsHabitCompletedTodayAsync(habit.Id);
            var currentStreak = await _habitService.GetHabitStreakAsync(habit.Id);

            var allCompletions = habit.Completions
                .OrderBy(c => c.CompletedDate).ToList();
            var longestStreak = CalculateLongestStreak(allCompletions);


            var completionHistory = new List<HabitCompletionDay>();
            for (int i = 29; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var completed = habit.Completions.Any(c => c.CompletedDate.Date == date);
                completionHistory.Add(new HabitCompletionDay
                {
                    Date = date,
                    IsCompleted = completed
                });
            }

            var viewModel = new HabitDetailsViewModel
            {
                Id = habit.Id,
                Title = habit.Title,
                Description = habit.Description,
                IconName = habit.IconName,
                Color = habit.Color,
                IsActive = habit.IsActive,
                CreatedAt = habit.CreatedAt,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                TotalCompletions = habit.Completions.Count,
                IsCompletedToday = isCompletedToday,
                CompletionHistory = completionHistory
            };

            return View(viewModel);
        }

        // GET: habits/create
        [Route("habits/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: habits/create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("habits/create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                await _habitService.CreateHabitAsync(model, userId);
                TempData["SuccessMessage"] = "Habit created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: habits/edit/5
        [Route("habits/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var habit = await _habitService.GetHabitByIdAsync(id, userId);

            if (habit == null)
            {
                return NotFound();
            }

            var customDays = new List<int>();
            if (habit.RepeatType == RepeatType.Custom && !string.IsNullOrEmpty(habit.CustomDays))
            {
                try
                {
                    var parsedDays = System.Text.Json.JsonSerializer.Deserialize<int[]>(habit.CustomDays);
                    if (parsedDays != null)
                    {
                        customDays = parsedDays.ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing custom days: {ex.Message}");
                    throw;
                }
            }

            var viewModel = new EditHabitViewModel
            {
                Id = habit.Id,
                Title = habit.Title,
                Description = habit.Description,
                IconName = habit.IconName,
                Color = habit.Color,
                IsActive = habit.IsActive,
                RepeatType = habit.RepeatType,
                CustomDays = customDays
            };

            return View(viewModel);
        }

        // POST: habits/edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("habits/edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditHabitViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                var success = await _habitService.UpdateHabitAsync(id, model, userId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Habit updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the habit.");
                    // return NotFound();
                }
            }

            return View(model);
        }

        // GET: habits/delete/5
        [Route("habits/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var habit = await _habitService.GetHabitByIdAsync(id, userId);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // POST: habits/delete/5
        [Route("habits/delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _habitService.DeleteHabitAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Habit deleted successfully!";
                //return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while deleting the habit.");
                //return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [Route("habits/toggle-completion")]
        [HttpPost]
        public async Task<IActionResult> ToggleCompletion(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var isCompleted = await _habitService.IsHabitCompletedTodayAsync(id);

            bool success;

            if (isCompleted)
            {
                success = await _habitService.UnmarkHabitCompleteAsync(id, userId);
            }
            else
            {
                success = await _habitService.MarkHabitCompleteAsync(id, userId);
            }

            if (success)
            {
                return Json(new { success = true, completed = !isCompleted });
            }

            return Json(new { success = false });
        }

        private async Task<bool> HabitExists(int id)
        {
            var habits = await _habitService.GetUserHabitsAsync(_userManager.GetUserId(User)!);
            return habits.Any(h => h.Id == id);
        }

        [Route("habits/toggle-status/{id}")]
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var userId = _userManager.GetUserId(User)!;
            var habit = await _habitService.GetHabitByIdAsync(id, userId);

            if (habit == null)
            {
                return Json(new { success = false, message = "Habit not found" });
            }

            var editModel = new EditHabitViewModel
            {
                Id = habit.Id,
                Title = habit.Title,
                Description = habit.Description,
                IconName = habit.IconName,
                Color = habit.Color,
                IsActive = isActive
            };

            var success = await _habitService.UpdateHabitAsync(id, editModel, userId);

            if (success)
            {
                var statusText = isActive ? "resumed" : "paused";
                return Json(new { success = true, message = $"Habit {statusText} successfully", isActive = isActive });
            }

            return Json(new { success = false, message = "Failed to update habit status" });
        }

        private int CalculateLongestStreak(List<HabitCompletion> completions)
        {
            if (!completions.Any()) return 0;

            var dates = completions.Select(c => c.CompletedDate.Date).Distinct().OrderBy(d => d).ToList();

            int longestStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i] == dates[i - 1].AddDays(1))
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }
    }
}
