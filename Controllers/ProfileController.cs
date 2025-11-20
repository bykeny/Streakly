using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HabitGoalTrackerApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Get user statistics
            var totalHabits = await _context.Habits.CountAsync(h => h.UserId == user.Id);
            var totalGoals = await _context.Goals.CountAsync(g => g.UserId == user.Id);
            var completedGoals = await _context.Goals.CountAsync(g => g.UserId == user.Id && g.CompletedAt != null);

            // Calculate current streak (simplified)
            var habits = await _context.Habits
                .Where(h => h.UserId == user.Id && h.IsActive)
                .Include(h => h.Completions)
                .ToListAsync();

            var currentStreak = CalculateCurrentStreak(habits);

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                TimeZone = user.TimeZone,
                DateFormat = user.DateFormat,
                EmailNotifications = user.EmailNotifications,
                DailyReminders = user.DailyReminders,
                WeeklyReports = user.WeeklyReports,
                ProfileImagePath = user.ProfileImagePath,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                TotalHabits = totalHabits,
                TotalGoals = totalGoals,
                CompletedGoals = completedGoals,
                CurrentStreak = currentStreak
            };

            return View(viewModel);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                TimeZone = user.TimeZone,
                DateFormat = user.DateFormat,
                EmailNotifications = user.EmailNotifications,
                DailyReminders = user.DailyReminders,
                WeeklyReports = user.WeeklyReports,
                ProfileImagePath = user.ProfileImagePath
            };

            return View(viewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Handle profile image deletion
            if (model.DeleteProfileImage && !string.IsNullOrEmpty(user.ProfileImagePath))
            {
                // Delete the physical file
                var profilesPath = Path.Combine(_environment.WebRootPath, "images", "profiles");
                var fileName = Path.GetFileName(user.ProfileImagePath);
                var filePath = Path.Combine(profilesPath, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Clear the profile image path
                user.ProfileImagePath = null;
            }
            // Handle profile image upload (only if not deleting)
            else if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.ProfileImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ProfileImageFile", "Only JPEG and PNG files are allowed.");
                    return View(model);
                }

                // Validate file size (max 2MB)
                if (model.ProfileImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfileImageFile", "File size must be less than 2MB.");
                    return View(model);
                }

                // Create directory if it doesn't exist
                var profilesPath = Path.Combine(_environment.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(profilesPath);

                // Generate unique filename
                var fileName = $"{user.Id}.jpg";
                var filePath = Path.Combine(profilesPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                // Update user profile image path
                user.ProfileImagePath = $"/images/profiles/{fileName}";
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;
            user.TimeZone = model.TimeZone;
            user.DateFormat = model.DateFormat;
            user.EmailNotifications = model.EmailNotifications;
            user.DailyReminders = model.DailyReminders;
            user.WeeklyReports = model.WeeklyReports;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Update email if changed
                if (user.Email != model.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (setEmailResult.Succeeded)
                    {
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                        if (!setUserNameResult.Succeeded)
                        {
                            foreach (var error in setUserNameResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }
                    else
                    {
                        foreach (var error in setEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        private int CalculateCurrentStreak(List<Habit> habits)
        {
            if (!habits.Any()) return 0;

            var currentDate = DateTime.Today;
            var streak = 0;

            while (true)
            {
                var scheduledHabits = habits.Where(h => h.IsScheduledForDate(currentDate)).ToList();
                if (!scheduledHabits.Any())
                {
                    currentDate = currentDate.AddDays(-1);
                    continue;
                }

                var completedHabits = scheduledHabits.Where(h => h.Completions.Any(c => c.CompletedDate.Date == currentDate)).Count();
                var completionRate = (decimal)completedHabits / scheduledHabits.Count;

                if (completionRate >= 0.8m) // 80% completion rate required for streak
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }
    }
}
