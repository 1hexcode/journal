using Journal.Dialogs;
using Journal.Repository;

namespace Journal.Components.Pages;

using MudBlazor;
using Microsoft.JSInterop;


public partial class JournalPage
{
    public const string Route = "/journal";
    
    // State
    private DateTime? SelectedDate { get; set; } = DateTime.Today;
    private string? SelectedPrimaryMood { get; set; }
    private IReadOnlyCollection<string> SelectedMoods { get; set; } = [];
    private string? NewMood { get; set; }
    private string HtmlContent = "";
    private bool _editorInitialized;


    // Data
    private List<string> MoodCategories { get; set; } = [];
    private List<string> SecondaryMoods { get; set; } = [];
    private List<Models.Journal> RecentEntries { get; set; } = [];
    
    // Class
    private class JournalEntry
    {
        public DateTime Date { get; set; }
        public string Html { get; set; } = "";
        public IReadOnlyCollection<string> Moods { get; set; } = new List<string>();
        
    }
    

    protected override async Task OnInitializedAsync()
    {
        await LoadMoodCategories();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initQuill", "editor");
            _editorInitialized = true;
        }
    }
    private async Task LoadMoodCategories()
    {
        var moods = await MoodRepository.GetAllAsync();
        MoodCategories = moods
            .Select(m => m.MoodCategory)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    private async Task OnPrimaryMoodChanged(string category)
    {
        SelectedPrimaryMood = category;
        
        if (string.IsNullOrEmpty(category))
        {
            SecondaryMoods = [];
            return;
        }

        var moods = await MoodRepository.GetByCategoryAsync(category);
        SecondaryMoods = moods
            .Select(m => m.MoodName)
            .OrderBy(n => n)
            .ToList();
    }

    private void OnMoodsChanged(IReadOnlyCollection<string> moods)
    {
        SelectedMoods = moods;
    }

    private void RemoveMood(string mood)
    {
        SelectedMoods = SelectedMoods.Where(m => m != mood).ToList();
    }

    private void ClearPrimarySelection()
    {
        SelectedPrimaryMood = null;
        SecondaryMoods = [];
    }

    private void AddMood()
    {
        if (string.IsNullOrWhiteSpace(NewMood)) return;
        
        var currentList = SelectedMoods.ToList();
        if (!currentList.Contains(NewMood))
        {
            currentList.Add(NewMood);
            SelectedMoods = currentList;
        }
        NewMood = string.Empty;
    }

    private void OnDateChanged(DateTime? date)
    {
        SelectedDate = date;
    }
    
    private static Color GetCategoryColor(string category) => category?.ToLower() switch
    {
        "positive" => Color.Success,
        "negative" => Color.Error,
        "neutral" => Color.Default,
        "energy" => Color.Warning,
        "social" => Color.Info,
        "calm" => Color.Tertiary,
        _ => Color.Primary
    };
    
    private static string GetCategoryEmoji(string category) => category?.ToLower() switch
    {
        "positive" => "😊",
        "negative" => "😔",
        "neutral" => "😐",
        "energy" => "⚡",
        "social" => "👥",
        "calm" => "🧘",
        _ => "🏷️"
    };
    
    private async Task Clear()
    {
        // Clear editor
        await JS.InvokeVoidAsync("clearQuill");

        // Clear moods
        SelectedMoods = new List<string>();

        // Optional: clear HTML cache
        HtmlContent = string.Empty;
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
        
        
        if (result.Canceled == false) 
        {
            await Save(); // user clicked Approve
        }
    }
    private async Task LoadRecentEntries()
    {
        RecentEntries = await JournalRepository.GetRecentAsync(10);
        StateHasChanged();
    }
    
    private async Task Save()
    {
        HtmlContent = await JS.InvokeAsync<string>("getQuillHtml");
        
        var html = await GetEditorHtml();
        var plainText = await JS.InvokeAsync<string>("quillInterop.getText");

        // Ensure one entry per date
        var entry = new Models.Journal
        {
            Id = Guid.NewGuid(),
            Date = SelectedDate ?? DateTime.Today,
            PrimaryMood = SelectedPrimaryMood ?? string.Empty,
            SecondaryMoods = string.Join(",", SelectedMoods),  // "Happy,Excited"
            Category = "General",
            Notes = html,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await JournalRepository.SaveAsync(entry);
        await LoadRecentEntries();
        await Clear();

        // Console.WriteLine(allEntriesHtml); // or display in UI
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

            if (entry.PrimaryMood.Any())
            {
                htmlBuilder.AppendLine("<p>Moods: " + string.Join(", ", entry.PrimaryMood) + "</p>");
            }
            else
            {
                Console.WriteLine("Logger Journal 1!");
            }

            htmlBuilder.AppendLine($"<div class='entry-content'>{entry.Notes}</div>");
            htmlBuilder.AppendLine("</div>");
            htmlBuilder.AppendLine("<hr>");
        }

        return htmlBuilder.ToString();
    }
    
    private async Task SetEditorHtml(string html)
    {
        if (!_editorInitialized) return;
        await JS.InvokeVoidAsync("quillInterop.setHtml", html);
    }

    private async Task ClearEditor()
    {
        if (!_editorInitialized) return;
        await JS.InvokeVoidAsync("quillInterop.clear");
    }
    
    private async Task<string> GetEditorHtml()
    {
        if (!_editorInitialized) return string.Empty;
        return await JS.InvokeAsync<string>("quillInterop.getHtml");
    }

    /*
    private IReadOnlyCollection<string> SelectedMoods = new List<string>();

    private string NewMood = "";

    
    
    private readonly Dictionary<string, Color> MoodColors = new()
    {
        ["Negative"] = Color.Warning,      // yellow
        ["Positive"] = Color.Success,      // green
        ["Neutral"] = Color.Primary,       // soft blue
    };
    
    private Color GetMoodColor(string mood)
    {
        return MoodColors.TryGetValue(mood, out var color)
            ? color
            : Color.Dark; // fallback for custom moods
    }


    

    

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
    */ 
}