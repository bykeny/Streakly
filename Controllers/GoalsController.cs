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
    public class GoalsController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsController(IGoalService goalService, UserManager<ApplicationUser> userManager)
        {
            _goalService = goalService;
            _userManager = userManager;
        }

        // GET: goals
        [Route("goals")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var goals = await _goalService.GetUserGoalsAsync(userId);

            var viewModel = goals.Select(g => new GoalListViewModel
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                TargetValue = g.TargetValue,
                CurrentValue = g.CurrentValue,
                Unit = g.Unit,
                CreatedAt = g.CreatedAt,
                TargetDate = g.TargetDate,
                CompletedAt = g.CompletedAt,
                Category = g.Category,
                ProgressPercentage = g.ProgressPercentage,
                IsCompleted = g.IsCompleted,
                IsOverdue = g.IsOverdue,
                DaysRemaining = g.DaysRemaining,
                CategoryDisplay = g.GetCategoryDisplay(),
                StatusDisplay = g.GetStatusDisplay()
            }).ToList();

            return View(viewModel);
        }

        // GET: goals/details/5
        [Route("goals/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _goalService.GetGoalByIdAsync(id, userId);

            if (goal == null)
            {
                return NotFound();
            }

            var statistics = await _goalService.GetGoalStatisticsAsync(id, userId);

            var progressEntries = new List<GoalProgressViewModel>();
            decimal runningTotal = 0;

            foreach (var entry in goal.ProgressEntries.OrderBy(p => p.CreatedAt))
            {
                runningTotal += entry.Value;
                progressEntries.Add(new GoalProgressViewModel
                {
                    Id = entry.Id,
                    Value = entry.Value,
                    Notes = entry.Notes,
                    CreatedAt = entry.CreatedAt,
                    RunningTotal = runningTotal
                });
            }

            var viewModel = new GoalDetailsViewModel
            {
                Id = goal.Id,
                Title = goal.Title,
                Description = goal.Description,
                TargetValue = goal.TargetValue,
                CurrentValue = goal.CurrentValue,
                Unit = goal.Unit,
                CreatedAt = goal.CreatedAt,
                TargetDate = goal.TargetDate,
                CompletedAt = goal.CompletedAt,
                Category = goal.Category,
                ProgressPercentage = goal.ProgressPercentage,
                IsCompleted = goal.IsCompleted,
                IsOverdue = goal.IsOverdue,
                DaysRemaining = goal.DaysRemaining,
                CategoryDisplay = goal.GetCategoryDisplay(),
                StatusDisplay = goal.GetStatusDisplay(),
                ProgressEntries = progressEntries.OrderByDescending(p => p.CreatedAt).ToList(),
                Statistics = statistics
            };

            return View(viewModel);
        }

        // GET: goals/create
        [Route("goals/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: goals/create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("goals/create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGoalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User)!;
                    await _goalService.CreateGoalAsync(model, userId);
                    TempData["SuccessMessage"] = "Goal created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    // Log the exception
                    ModelState.AddModelError("", "An error occurred while creating the goal. Please try again.");
                }
            }
            return View(model);
        }

        // GET: goals/edit/5
        [Route("goals/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _goalService.GetGoalByIdAsync(id, userId);

            if (goal == null)
            {
                return NotFound();
            }

            var viewModel = new EditGoalViewModel
            {
                Id = goal.Id,
                Title = goal.Title,
                Description = goal.Description,
                TargetValue = goal.TargetValue,
                Unit = goal.Unit,
                TargetDate = goal.TargetDate,
                Category = goal.Category
            };

            return View(viewModel);
        }

        // POST: goals/edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("goals/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditGoalViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                var success = await _goalService.UpdateGoalAsync(id, model, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Goal updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        // GET: goals/delete/5
        [Route("goals/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal = await _goalService.GetGoalByIdAsync(id, userId);

            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // POST: goals/delete/5
        [Route("goals/delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _goalService.DeleteGoalAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Goal deleted successfully!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        //private bool GoalExists(int id)
        //{
        //    return _context.Goals.Any(e => e.Id == id);
        //}

        // POST: goals/add-progress/5
        [Route("goals/add-progress")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProgress(int id, AddProgressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                var success = await _goalService.AddProgressAsync(id, model, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Progress added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add progress. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid progress data. Please check your input.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Goals/DeleteProgress/5
        [HttpPost]
        //public async Task<IActionResult> DeleteProgress(int id)
        //{
        //    var userId = _userManager.GetUserId(User)!;
        //    var success = await _goalService.DeleteProgressAsync(id, userId);

        //    if (success)
        //    {
        //        return Json(new { success = true, message = "Progress entry deleted successfully!" });
        //    }

        //    return Json(new { success = false, message = "Failed to delete progress entry." });
        //}

        [Route("goals/delete-progress")]
        public async Task<IActionResult> DeleteProgress(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _goalService.DeleteProgressAsync(id, userId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (success)
                {
                    return Json(new { success = true, message = "Progress entry deleted successfully!" });
                }
                return Json(new { success = false, message = "Failed to delete progress entry." });
            }

            // For non-AJAX requests
            if (success)
            {
                TempData["SuccessMessage"] = "Progress entry deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete progress entry.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
