using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;

namespace HabitGoalTrackerApp.Services.Interfaces
{
    public interface IJournalService
    {
        Task<IEnumerable<JournalEntryListViewModel>> GetUserJournalEntriesAsync(string userId, int page = 1, int pageSize = 10, string? searchTerm = null, string? tag = null);
        Task<JournalEntryDetailsViewModel?> GetJournalEntryByIdAsync(int id, string userId);
        Task<JournalEntry> CreateJournalEntryAsync(CreateJournalEntryViewModel model, string userId);
        Task<bool> UpdateJournalEntryAsync(int id, EditJournalEntryViewModel model, string userId);
        Task<bool> DeleteJournalEntryAsync(int id, string userId);
        Task<bool> ToggleFavoriteAsync(int id, string userId);
        Task<JournalStatsViewModel> GetJournalStatsAsync(string userId);
        Task<List<string>> GetUserTagsAsync(string userId);
    }
}
