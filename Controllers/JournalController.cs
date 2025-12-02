using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabitGoalTrackerApp.Controllers
{
    [Authorize]
    public class JournalController : Controller
    {
        private readonly IJournalService _journalService;
        private readonly UserManager<ApplicationUser> _userManager;

        public JournalController(IJournalService journalService, UserManager<ApplicationUser> userManager)
        {
            _journalService = journalService;
            _userManager = userManager;
        }

        // GET: journal
        [Route("journal")]
        public async Task<IActionResult> Index(int page = 1, string? search = null, string? tag = null)
        {
            var userId = _userManager.GetUserId(User)!;
            var entries = await _journalService.GetUserJournalEntriesAsync(userId, page, 12, search, tag);
            var stats = await _journalService.GetJournalStatsAsync(userId);
            var userTags = await _journalService.GetUserTagsAsync(userId);

            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = search;
            ViewBag.SelectedTag = tag;
            ViewBag.UserTags = userTags;
            ViewBag.Stats = stats;

            return View(entries);
        }

        // GET: journal/details/5
        [Route("journal/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var entry = await _journalService.GetJournalEntryByIdAsync(id, userId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // GET: journal/create
        [Route("journal/create")]
        public IActionResult Create()
        {
            var model = new CreateJournalEntryViewModel
            {
                Title = $"Journal Entry - {DateTime.Today:MMM dd, yyyy}"
            };
            return View(model);
        }

        // POST: journal/create
        [Route("journal/create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJournalEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                var entry = await _journalService.CreateJournalEntryAsync(model, userId);
                TempData["SuccessMessage"] = "Journal entry created successfully!";
                return RedirectToAction(nameof(Details), new { id = entry.Id });
            }
            return View(model);
        }

        // GET: journal/edit/5
        [Route("journal/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var entry = await _journalService.GetJournalEntryByIdAsync(id, userId);

            if (entry == null)
            {
                return NotFound();
            }

            var model = new EditJournalEntryViewModel
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                Mood = entry.Mood,
                TagsInput = string.Join(", ", entry.Tags),
                IsFavorite = entry.IsFavorite
            };

            return View(model);
        }

        // POST: journal/edit/5
        [Route("journal/edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditJournalEntryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User)!;
                var success = await _journalService.UpdateJournalEntryAsync(id, model, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Journal entry updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                else
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        // GET: journal/delete/5
        [Route("journal/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var entry = await _journalService.GetJournalEntryByIdAsync(id, userId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // POST: journal/delete/5
        [Route("journal/delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _journalService.DeleteJournalEntryAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Journal entry deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: journal/toggle-favorite/5
        [Route("journal/toggle-favorite/{id}")]
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _journalService.ToggleFavoriteAsync(id, userId);

            if (success)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: journal/stats
        [Route("journal/stats")]
        public async Task<IActionResult> Stats()
        {
            var userId = _userManager.GetUserId(User)!;
            var stats = await _journalService.GetJournalStatsAsync(userId);
            return View(stats);
        }
    }
}
