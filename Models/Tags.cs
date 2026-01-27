using System.Text.Json;
using SQLite;

namespace Journal.Models;

public class Tags: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    
    public string TagName {get; set;}
    
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;

    public object Clone()
    {
        return new Tags
        {
            Id = Id,
            TagName = TagName,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}