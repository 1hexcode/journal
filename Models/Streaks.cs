using System.Text.Json;
using SQLite;

namespace Journal.Models;

public class Streaks: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    public int CurrentCount { get; set; }
    public int LongestCount { get; set; }
    public int TotalEntries { get; set; }
    public DateTime LastEntryDate { get; set; }
    public int NextMilestone { get; set; }
    public int PreviousMilestone { get; set; } 
    public string NextMilestoneName { get; set; } = "";
    public int DaysToNextMilestone { get; set; }
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;
    public bool IsStreakActive { get; set; }

    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public object Clone()
    {
        return new Streaks
        {
            Id = Id,
            CurrentCount = CurrentCount,
            LongestCount = LongestCount,
            TotalEntries = TotalEntries,
            LastEntryDate = LastEntryDate,
            NextMilestone = NextMilestone,
            PreviousMilestone = PreviousMilestone,
            NextMilestoneName = NextMilestoneName,
            IsStreakActive = IsStreakActive,
            DaysToNextMilestone = DaysToNextMilestone,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}

public class UserAchievement: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    
    public string Name { get; set; } // e.g., "Month Master"
    public string Description { get; set; }
    public int GoalValue { get; set; } // e.g., 30 for 30 days
    public int CurrentProgress { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? DateUnlocked { get; set; }
    
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public object Clone()
    {
        return new UserAchievement
        {
            Id = Id,
            Name = Name,
            Description = Description,
            GoalValue = GoalValue,
            CurrentProgress = CurrentProgress,
            DateUnlocked = DateUnlocked,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}

public class ActivityDay: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    
    // The date of activity
    public DateTime Date { get; set; }

    // In your image, the dots are green. 
    // You can use a count to determine the "green-ness" (intensity).
    public int EntryCount { get; set; }

    public object Clone()
    {
        return new ActivityDay
        {
            Id = Id,
            Date = Date,
            EntryCount = EntryCount,
        };
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}