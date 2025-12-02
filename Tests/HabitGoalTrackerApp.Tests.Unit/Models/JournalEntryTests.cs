using FluentAssertions;
using HabitGoalTrackerApp.Models;

namespace HabitGoalTrackerApp.Tests.Unit.Models;

public class JournalEntryTests
{
    [Fact]
    public void GetMoodEmoji_ShouldReturnCorrectEmoji()
    {
        // Arrange & Act & Assert
        new JournalEntry { Mood = JournalMood.VeryHappy }.GetMoodEmoji().Should().Be("😄");
        new JournalEntry { Mood = JournalMood.Happy }.GetMoodEmoji().Should().Be("😊");
        new JournalEntry { Mood = JournalMood.Neutral }.GetMoodEmoji().Should().Be("😐");
        new JournalEntry { Mood = JournalMood.Sad }.GetMoodEmoji().Should().Be("😔");
        new JournalEntry { Mood = JournalMood.VerySad }.GetMoodEmoji().Should().Be("😢");
    }

    [Fact]
    public void GetMoodColor_ShouldReturnCorrectColor()
    {
        // Arrange & Act & Assert
        new JournalEntry { Mood = JournalMood.VeryHappy }.GetMoodColor().Should().Be("#22c55e");
        new JournalEntry { Mood = JournalMood.Happy }.GetMoodColor().Should().Be("#84cc16");
        new JournalEntry { Mood = JournalMood.Neutral }.GetMoodColor().Should().Be("#6b7280");
        new JournalEntry { Mood = JournalMood.Sad }.GetMoodColor().Should().Be("#f59e0b");
        new JournalEntry { Mood = JournalMood.VerySad }.GetMoodColor().Should().Be("#ef4444");
    }
}
