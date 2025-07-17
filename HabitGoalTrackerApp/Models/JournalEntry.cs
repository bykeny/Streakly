using System.ComponentModel.DataAnnotations;

namespace HabitGoalTrackerApp.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public JournalMood Mood { get; set; } = JournalMood.Neutral;

        [StringLength(500)]
        public string? Tags { get; set; } // Comma-separated tags

        public bool IsFavorite { get; set; } = false;

        // Foreign key
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        // Helper methods
        public List<string> GetTags()
        {
            if (string.IsNullOrEmpty(Tags))
                return new List<string>();

            return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(tag => tag.Trim())
                      .Where(tag => !string.IsNullOrEmpty(tag))
                      .ToList();
        }

        public void SetTags(List<string> tags)
        {
            Tags = string.Join(", ", tags.Where(tag => !string.IsNullOrEmpty(tag?.Trim())));
        }

        public string GetMoodEmoji()
        {
            return Mood switch
            {
                JournalMood.VeryHappy => "😄",
                JournalMood.Happy => "😊",
                JournalMood.Neutral => "😐",
                JournalMood.Sad => "😔",
                JournalMood.VerySad => "😢",
                _ => "😐"
            };
        }

        public string GetMoodColor()
        {
            return Mood switch
            {
                JournalMood.VeryHappy => "#22c55e",
                JournalMood.Happy => "#84cc16",
                JournalMood.Neutral => "#6b7280",
                JournalMood.Sad => "#f59e0b",
                JournalMood.VerySad => "#ef4444",
                _ => "#6b7280"
            };
        }
    }

    public enum JournalMood
    {
        VerySad = 1,
        Sad = 2,
        Neutral = 3,
        Happy = 4,
        VeryHappy = 5
    }
}
