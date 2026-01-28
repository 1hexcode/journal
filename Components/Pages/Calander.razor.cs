namespace Journal.Components.Pages;
using MudBlazor;

public partial class Calander
{
    private DateTime currentDate = DateTime.Today;
    private DateTime? selectedDate = null;
    private string currentView = "month";
    private Dictionary<DateTime, JournalEntryPreview> entriesByDate = new();

    protected override void OnInitialized()
    {
        if (!AuthService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        LoadEntries();
    }

    private async Task LoadEntries()
    {
        try
        {
            // Get all entries from database
            var allEntries = await JournalRepository.GetAllAsync();
        
            // Clear existing entries
            entriesByDate.Clear();
        
            // Group entries by date (in case multiple entries per day)
            foreach (var entry in allEntries)
            {
                var dateOnly = entry.Date.Date; // Get just the date part
            
                if (!entriesByDate.ContainsKey(dateOnly))
                {
                    entriesByDate[dateOnly] = new JournalEntryPreview
                    {
                        Date = dateOnly,
                        Mood = entry.PrimaryMood,
                        Preview = entry.Title,
                        EntryId = entry.Id
                    };
                }
            }
        
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading entries: {ex.Message}");
            // Optionally show error to user
        }
    }

    private string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;
    
        // Basic HTML stripping
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty).Trim();
    }

    private List<DateTime> GetCalendarDays()
    {
        var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        
        var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);
        var endDate = lastDay.AddDays(6 - (int)lastDay.DayOfWeek);
        
        var days = new List<DateTime>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            days.Add(date);
        }
        return days;
    }

    private List<DateTime> GetMiniCalendarDays(DateTime month)
    {
        var firstDay = new DateTime(month.Year, month.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
        
        var days = new List<DateTime>();
        for (int i = 1; i <= daysInMonth; i++)
        {
            days.Add(new DateTime(month.Year, month.Month, i));
        }
        return days;
    }

    private string GetDayClass(DateTime day, bool isToday, bool isCurrentMonth, bool hasEntry)
    {
        var classes = new List<string> { "calendar-day" };
        
        if (isToday) classes.Add("calendar-day-today");
        if (!isCurrentMonth) classes.Add("calendar-day-other-month");
        if (hasEntry) classes.Add("calendar-day-has-entry");
        
        return string.Join(" ", classes);
    }

    private string GetMiniDayClass(bool hasEntry, bool isToday)
    {
        var classes = new List<string> { "mini-day" };
        if (hasEntry) classes.Add("mini-day-entry");
        if (isToday) classes.Add("mini-day-today");
        return string.Join(" ", classes);
    }

    private Color GetMoodColor(string mood)
    {
        if (mood.Contains("Positive") || mood.Contains("Grateful")) return MudBlazor.Color.Success;
        if (mood.Contains("Negative") || mood.Contains("Frustrated")) return MudBlazor.Color.Error;
        if (mood.Contains("Neutral")) return MudBlazor.Color.Info;
        return MudBlazor.Color.Default;
    }

    private void PreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);
    }

    private void NextMonth()
    {
        currentDate = currentDate.AddMonths(1);
    }

    private void GoToToday()
    {
        currentDate = DateTime.Today;
        selectedDate = DateTime.Today;
    }

    private void SetView(string view)
    {
        currentView = view;
        if (view == "year")
        {
            currentDate = new DateTime(DateTime.Today.Year, 1, 1);
        }
    }

    private void OnDayClick(DateTime day)
    {
        selectedDate = day;
    }

    private void OnMiniDayClick(DateTime day)
    {
        currentDate = day;
        selectedDate = day;
        currentView = "month";
    }

    private int GetMonthEntryCount()
    {
        return entriesByDate.Values.Count(e => 
            e.Date.Year == currentDate.Year && 
            e.Date.Month == currentDate.Month);
    }

    private void ViewEntry(JournalEntryPreview entry)
    {
        Snackbar.Add($"Viewing entry from {entry.Date:MMM dd}", MudBlazor.Severity.Info);
        NavigationManager.NavigateTo($"/journal/view/{entry.EntryId}");
    }

    private void EditEntry(JournalEntryPreview entry)
    {
        Snackbar.Add($"Editing entry from {entry.Date:MMM dd}", MudBlazor.Severity.Info);
        NavigationManager.NavigateTo($"/journal/view/{entry.EntryId}");
    }

    private void CreateEntry(DateTime date)
    {
        NavigationManager.NavigateTo("/journal");
    }
    
    private string GetMoodEmoji(string mood)
    {
        if (string.IsNullOrEmpty(mood)) return "";
    
        if (mood.Contains("Positive") || mood.Contains("positive")) return "😊";
        if (mood.Contains("Negative") || mood.Contains("negative")) return "😔";
        if (mood.Contains("Frustrated") || mood.Contains("frustrated")) return "😤";
        if (mood.Contains("Neutral") || mood.Contains("neutral")) return "🤗";
        
        return "📝"; // Default
    }

    public class JournalEntryPreview
    {
        public DateTime Date { get; set; }
        public string Mood { get; set; }
        public string Preview { get; set; }
        
        public Guid EntryId { get; set; } 
    }
}