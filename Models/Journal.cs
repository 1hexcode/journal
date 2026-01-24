using System.Text.Json;

namespace Journal.Models;

// Enums
public enum MoodCategory
{
    Positive,
    Neutral,
    Negative
}

public enum PredefinedMood
{
    // Positive
    Happy,
    Excited,
    Relaxed,
    Grateful,
    Confident,
    
    // Neutral
    Calm,
    Thoughtful,
    Curious,
    Nostalgic,
    Bored,
    
    // Negative
    Sad,
    Angry,
    Stressed,
    Lonely,
    Anxious
}

public enum PredefinedTag
{
    Work,
    Career,
    Studies,
    Family,
    Friends,
    Relationships,
    Health,
    Fitness,
    PersonalGrowth,
    SelfCare,
    Hobbies,
    Travel,
    Nature,
    Finance,
    Spirituality,
    Birthday,
    Holiday,
    Vacation,
    Celebration,
    Exercise,
    Reading,
    Writing,
    Cooking,
    Meditation,
    Yoga,
    Music,
    Shopping,
    Parenting,
    Projects,
    Planning,
    Reflection
}

public class Journal: IModel, ICloneable
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; } // Only date part (one entry per day)
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Moods
    public PredefinedMood PrimaryMood { get; set; } // Required
    public List<PredefinedMood>? SecondaryMoods { get; set; } // Max 2
    
    // Category
    public MoodCategory Category { get; set; }
    
    // Content
    public string? Notes { get; set; }
    
    // Tags
    public List<PredefinedTag>? PredefinedTags { get; set; }
    public List<string>? CustomTags { get; set; }

    public object Clone()
    {
        return new Journal
        {
            Id = Id,
            UserId = UserId,
            Date = Date,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            Category = Category,
            Notes = Notes,
            PrimaryMood = PrimaryMood,
            SecondaryMoods = SecondaryMoods,
            PredefinedTags = PredefinedTags,
            CustomTags = CustomTags
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}