using Journal.Dialogs;

namespace Journal.Components.Pages;

using MudBlazor;
using Microsoft.JSInterop;


public partial class JournalPage
{
    public const string Route = "/journal";
    
    /* -----------------------------
       STATE
    ------------------------------*/

    private DateTime SelectedDate = DateTime.Today;

    private string HtmlContent = "";

    private List<string> AvailableMoods = new()
    {
        "Great", "Good", "Okay", "Sad", "Terrible", "Normal"
    };

    private IReadOnlyCollection<string> SelectedMoods = new List<string>();

    private string NewMood = "";

    private List<JournalEntry> RecentEntries = new();
    
    private readonly Dictionary<string, Color> MoodColors = new()
    {
        ["Great"] = Color.Warning,     // yellow
        ["Good"] = Color.Info,         // blue
        ["Okay"] = Color.Success, // green
        ["Sad"] = Color.Primary,       // soft blue
        ["Terrible"] = Color.Error,     // red
        ["Normal"] = Color.Secondary    // purple/gray
    };
    
    private Color GetMoodColor(string mood)
    {
        return MoodColors.TryGetValue(mood, out var color)
            ? color
            : Color.Dark; // fallback for custom moods
    }
    /* -----------------------------
       LIFECYCLE
    ------------------------------*/

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initQuill", "editor");
        }
    }

    /* -----------------------------
       HANDLERS
    ------------------------------*/

    private void OnDateChanged(DateTime? date)
    {
        if (date.HasValue)
            SelectedDate = date.Value;
    }

    private void OnMoodsChanged(IReadOnlyCollection<string> values)
    {
        SelectedMoods = values;
    }

    private void AddMood()
    {
        if (string.IsNullOrWhiteSpace(NewMood))
            return;

        var mood = NewMood.Trim();

        if (!AvailableMoods.Contains(mood))
            AvailableMoods.Add(mood);

        var updated = SelectedMoods.ToList(); updated.Add(mood); 
        SelectedMoods = updated;
        NewMood = "";
    }
    
    private async Task Clear()
    {
        // Clear editor
        await JS.InvokeVoidAsync("clearQuill");

        // Clear moods
        SelectedMoods = new List<string>();

        // Optional: clear HTML cache
        HtmlContent = string.Empty;
    }

    /* -----------------------------
       MODEL
    ------------------------------*/

    private class JournalEntry
    {
        public DateTime Date { get; set; }
        public string Html { get; set; } = "";
        public IReadOnlyCollection<string> Moods { get; set; } = new List<string>();
        
    }
    
    private string GetAllEntriesHtml()
    {
        if (!RecentEntries.Any())
            return "<p>No entries yet.</p>";

        // Build HTML string
        var htmlBuilder = new System.Text.StringBuilder();

        foreach (var entry in RecentEntries.OrderBy(e => e.Date))
        {
            htmlBuilder.AppendLine($"<div class='journal-entry'>");
            htmlBuilder.AppendLine($"<h3>{entry.Date:dd MMM yyyy}</h3>");

            if (entry.Moods.Any())
            {
                htmlBuilder.AppendLine("<p>Moods: " + string.Join(", ", entry.Moods) + "</p>");
            }
            else
            {
                Console.WriteLine("Fuck!");
            }

            htmlBuilder.AppendLine($"<div class='entry-content'>{entry.Html}</div>");
            htmlBuilder.AppendLine("</div>");
            htmlBuilder.AppendLine("<hr>");
        }

        return htmlBuilder.ToString();
    }
    
    private async Task Save()
    {
        HtmlContent = await JS.InvokeAsync<string>("getQuillHtml");

        // Ensure one entry per date
        RecentEntries.RemoveAll(e => e.Date.Date == SelectedDate.Date);

        RecentEntries.Add(new JournalEntry
        {
            Date = SelectedDate,
            Html = HtmlContent,
            Moods = SelectedMoods.ToList()
        });

        // For demonstration: get all entries in HTML
        var allEntriesHtml = GetAllEntriesHtml();

        Console.WriteLine(allEntriesHtml); // or display in UI
    }
    
    private async Task SaveWithConfirmation()
    {
        // Dialog parameters
        var parameters = new DialogParameters
        {
            ["Message"] = "Do you want to save this journal entry?"
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };

        // Show dialog
        var dialog = DialogService.Show<ApproveDialog>("Confirm Save", parameters, options);
        var result = await dialog.Result;
        

        // NEW MudBlazor (6.x+)
        if (result.Canceled == false) 
        {
            await Save(); // user clicked Approve
        }
    }

}