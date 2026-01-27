using System.Text.Json;
using SQLite;

namespace Journal.Models;

public class Mood: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    public string MoodName { get; set; }
    public string MoodCategory { get; set; }
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;

    public object Clone()
    {
        return new Mood
        {
            Id = Id,
            MoodName = MoodName,
            MoodCategory = MoodCategory,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}