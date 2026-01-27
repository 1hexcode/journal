using System.Text.Json;
using SQLite;

namespace Journal.Models;

public class Tags: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    
    public string MoodId {get; set;}
    public string TagId {get; set;}
    
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;

    public object Clone()
    {
        return new Tags
        {
            Id = Id,
            MoodId = MoodId,
            TagId = TagId,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}