namespace Journal.Components.Pages;

public partial class Reminder
{
    public const string Route = "/reminders";
    
    private bool remindersEnabled = false;
    private TimeSpan? reminderTime = new TimeSpan(20, 0, 0); // 8:00 PM default
    private string frequency = "daily";
    private string notificationStyle = "motivational";
    private bool isSaving = false;
    
    private List<string> daysOfWeek = new() 
    { 
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" 
    };
    
    private HashSet<string> selectedDays = new() 
    { 
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" 
    };

    protected override void OnInitialized()
    {
        if (!AuthService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        LoadReminderSettings();
    }

    private void LoadReminderSettings()
    {
     
        // For now using defaults
    }

    private void ToggleDay(string day)
    {
        if (selectedDays.Contains(day))
            selectedDays.Remove(day);
        else
            selectedDays.Add(day);
    }

    private async Task SaveReminders()
    {
        isSaving = true;

        try
        {
            
            await Task.Delay(1000); // Simulate save

            Snackbar.Add("Reminder settings saved successfully!", MudBlazor.Severity.Success);
            
            if (remindersEnabled)
            {
                var time = reminderTime?.ToString(@"hh\:mm") ?? "Not set";
                var period = reminderTime?.Hours >= 12 ? "PM" : "AM";
                Snackbar.Add($"You'll receive reminders at {time} {period}", MudBlazor.Severity.Info);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving settings: {ex.Message}", MudBlazor.Severity.Error);
        }
        finally
        {
            isSaving = false;
        }
    }

    private string GetPreviewMessage()
    {
        return notificationStyle switch
        {
            "gentle" => "It's time to write in your journal 📝",
            "motivational" => "Your future self will thank you for writing today! ✨",
            "prompt" => "What made you smile today? Share your thoughts 😊",
            _ => "Time to journal!"
        };
    }

    private string GetScheduleText()
    {
        if (!remindersEnabled) return "Reminders disabled";

        var time = reminderTime?.ToString(@"hh\:mm") ?? "Not set";
        var period = reminderTime?.Hours >= 12 ? "PM" : "AM";
        var formattedTime = $"{time} {period}";
    
        return frequency switch
        {
            "daily" => $"Every day at {formattedTime}",
            "weekdays" => $"Weekdays at {formattedTime}",
            "custom" => selectedDays.Any() 
                ? $"{string.Join(", ", selectedDays.Select(d => d.Substring(0, 3)))} at {formattedTime}"
                : "No days selected",
            _ => formattedTime
        };
    }
}