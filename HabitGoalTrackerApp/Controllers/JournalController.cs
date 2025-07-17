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

        // GET: Journal
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

        // GET: Journal/Details/5
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

        // GET: Journal/Create
        public IActionResult Create()
        {
            var model = new CreateJournalEntryViewModel
            {
                Title = $"Journal Entry - {DateTime.Today:MMM dd, yyyy}"
            };
            return View(model);
        }

        // POST: Journal/Create
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

        // GET: Journal/Edit/5
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

        // POST: Journal/Edit/5
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

        // GET: Journal/Delete/5
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

        // POST: Journal/Delete/5
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

        // POST: Journal/ToggleFavorite/5
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

        // GET: Journal/Stats
        public async Task<IActionResult> Stats()
        {
            var userId = _userManager.GetUserId(User)!;
            var stats = await _journalService.GetJournalStatsAsync(userId);
            return View(stats);
        }
    }
}
