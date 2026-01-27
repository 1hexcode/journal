using System.Text.Json;
using SQLite;

namespace Journal.Models;


public class Journal: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; } // Only date part (one entry per day)
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Moods
    public string PrimaryMood { get; set; } // Required
    public string SecondaryMoods { get; set; } // Max 2
    
    // Category
    public string Category { get; set; }
    
    // Content
    public string? Notes { get; set; }
    
    // Tags
    public string PredefinedTags { get; set; }
    public string CustomTags { get; set; }

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