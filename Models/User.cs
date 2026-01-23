using System.Text.Json;

namespace Journal.Models;

public class User: IModel, ICloneable
{
    public Guid Id {get; set;}
    public string Username {get; set;}
    public string Password {get; set;}
    public string Email {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public bool HasPassword { get; set; } = true;
    public DateTime CreatedAt {get; set;} =  DateTime.Now;
    
    
    public object Clone()
    {
        return new User
        {
            Id = Id,
            CreatedAt = CreatedAt,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            HasPassword = HasPassword,
            Password = Password,
            Username = Username
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}