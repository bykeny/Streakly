using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models.ViewModels
{
    public class JournalEntryListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public JournalMood Mood { get; set; }
        public string MoodEmoji { get; set; } = string.Empty;
        public string MoodColor { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public bool IsFavorite { get; set; }
        public int WordCount { get; set; }
    }

    public class JournalEntryDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public JournalMood Mood { get; set; }
        public string MoodEmoji { get; set; } = string.Empty;
        public string MoodColor { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public bool IsFavorite { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
    }

    public class CreateJournalEntryViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Mood")]
        public JournalMood Mood { get; set; } = JournalMood.Neutral;

        [Display(Name = "Tags")]
        public string? TagsInput { get; set; }

        [Display(Name = "Mark as Favorite")]
        public bool IsFavorite { get; set; } = false;
    }

    public class EditJournalEntryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Mood")]
        public JournalMood Mood { get; set; } = JournalMood.Neutral;

        [Display(Name = "Tags")]
        public string? TagsInput { get; set; }

        [Display(Name = "Mark as Favorite")]
        public bool IsFavorite { get; set; } = false;
    }

    public class JournalStatsViewModel
    {
        public int TotalEntries { get; set; }
        public int EntriesThisMonth { get; set; }
        public int FavoriteEntries { get; set; }
        public int TotalWords { get; set; }
        public double AverageWordsPerEntry { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public Dictionary<JournalMood, int> MoodDistribution { get; set; } = new();
        public List<string> PopularTags { get; set; } = new();
        public DateTime? LastEntryDate { get; set; }
    }
}
