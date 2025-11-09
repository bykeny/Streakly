using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitGoalTrackerApp.Services.Implementation
{
    public class JournalService : IJournalService
    {
        private readonly ApplicationDbContext _context;

        public JournalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JournalEntryListViewModel>> GetUserJournalEntriesAsync(
            string userId, int page = 1, int pageSize = 10, string? searchTerm = null, string? tag = null)
        {
            var query = _context.JournalEntries
                .Where(j => j.UserId == userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(j => j.Title.Contains(searchTerm) || j.Content.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(j => j.Tags != null && j.Tags.Contains(tag));
            }

            var entries = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entries.Select(j => new JournalEntryListViewModel
            {
                Id = j.Id,
                Title = j.Title,
                ContentPreview = GetContentPreview(j.Content, 150),
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                Mood = j.Mood,
                MoodEmoji = j.GetMoodEmoji(),
                MoodColor = j.GetMoodColor(),
                Tags = j.GetTags(),
                IsFavorite = j.IsFavorite,
                WordCount = CountWords(j.Content)
            });
        }

        public async Task<JournalEntryDetailsViewModel?> GetJournalEntryByIdAsync(int id, string userId)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (entry == null) return null;

            return new JournalEntryDetailsViewModel
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                Mood = entry.Mood,
                MoodEmoji = entry.GetMoodEmoji(),
                MoodColor = entry.GetMoodColor(),
                Tags = entry.GetTags(),
                IsFavorite = entry.IsFavorite,
                WordCount = CountWords(entry.Content),
                CharacterCount = entry.Content.Length
            };
        }

        public async Task<JournalEntry> CreateJournalEntryAsync(CreateJournalEntryViewModel model, string userId)
        {
            var entry = new JournalEntry
            {
                Title = model.Title,
                Content = model.Content,
                Mood = model.Mood,
                IsFavorite = model.IsFavorite,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(model.TagsInput))
            {
                var tags = model.TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();
                entry.SetTags(tags);
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<bool> UpdateJournalEntryAsync(int id, EditJournalEntryViewModel model, string userId)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (entry == null) return false;

            entry.Title = model.Title;
            entry.Content = model.Content;
            entry.Mood = model.Mood;
            entry.IsFavorite = model.IsFavorite;
            entry.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(model.TagsInput))
            {
                var tags = model.TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();
                entry.SetTags(tags);
            }
            else
            {
                entry.Tags = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJournalEntryAsync(int id, string userId)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (entry == null) return false;

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleFavoriteAsync(int id, string userId)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (entry == null) return false;

            entry.IsFavorite = !entry.IsFavorite;
            entry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JournalStatsViewModel> GetJournalStatsAsync(string userId)
        {
            var entries = await _context.JournalEntries
                .Where(j => j.UserId == userId)
                .ToListAsync();

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;

            var stats = new JournalStatsViewModel
            {
                TotalEntries = entries.Count,
                EntriesThisMonth = entries.Count(e => e.CreatedAt.Month == thisMonth && e.CreatedAt.Year == thisYear),
                FavoriteEntries = entries.Count(e => e.IsFavorite),
                TotalWords = entries.Sum(e => CountWords(e.Content)),
                LastEntryDate = entries.OrderByDescending(e => e.CreatedAt).FirstOrDefault()?.CreatedAt
            };

            stats.AverageWordsPerEntry = stats.TotalEntries > 0 ? (double)stats.TotalWords / stats.TotalEntries : 0;

            // Calculate mood distribution
            stats.MoodDistribution = entries
                .GroupBy(e => e.Mood)
                .ToDictionary(g => g.Key, g => g.Count());

            // Calculate streaks
            var streaks = CalculateStreaks(entries);
            stats.CurrentStreak = streaks.currentStreak;
            stats.LongestStreak = streaks.longestStreak;

            // Get popular tags
            var allTags = entries
                .SelectMany(e => e.GetTags())
                .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();

            stats.PopularTags = allTags;

            return stats;
        }

        public async Task<List<string>> GetUserTagsAsync(string userId)
        {
            var entries = await _context.JournalEntries
                .Where(j => j.UserId == userId && j.Tags != null)
                .Select(j => j.Tags)
                .ToListAsync();

            var allTags = entries
                .SelectMany(tags => tags!.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(tag => tag)
                .ToList();

            return allTags;
        }

        private string GetContentPreview(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            if (content.Length <= maxLength) return content;

            return content.Substring(0, maxLength) + "...";
        }

        private int CountWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private (int currentStreak, int longestStreak) CalculateStreaks(List<JournalEntry> entries)
        {
            if (!entries.Any()) return (0, 0);

            var entriesByDate = entries
                .GroupBy(e => e.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => g.Key)
                .ToList();

            var currentStreak = 0;
            var longestStreak = 0;
            var tempStreak = 0;

            var currentDate = DateTime.Today;
            var hasEntryToday = entriesByDate.Contains(currentDate);
            var checkDate = hasEntryToday ? currentDate : currentDate.AddDays(-1);

            // Calculate current streak
            while (entriesByDate.Contains(checkDate))
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }

            // Calculate longest streak
            DateTime? previousDate = null;
            foreach (var date in entriesByDate)
            {
                if (previousDate == null || date == previousDate.Value.AddDays(1))
                {
                    tempStreak++;
                    longestStreak = Math.Max(longestStreak, tempStreak);
                }
                else
                {
                    tempStreak = 1;
                }
                previousDate = date;
            }

            return (currentStreak, longestStreak);
        }
    }
}
