using System.Text.Json;
using SQLite;

namespace Journal.Models;

public class Reminder: IModel, ICloneable
{
    [PrimaryKey]
    public Guid Id {get; set;}
    
    public string ReminderTitle{get; set;}
    public string ReminderNotes {get; set;}
    public DateTime ReminderDate {get; set;}
    
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    public DateTime UpdatedAt {get; set;} =  DateTime.Now;

    public object Clone()
    {
        return new Reminder
        {
            Id = Id,
            ReminderTitle = ReminderTitle,
            ReminderNotes = ReminderNotes,
            ReminderDate = ReminderDate,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}