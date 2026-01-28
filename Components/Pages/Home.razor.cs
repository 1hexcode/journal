using Journal.Repository;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Journal.Components.Pages;

using Journal.Dialogs;

public partial class Home
{
    public const string Route = "/home";

    private const string StreakDialogLastShownKey = "StreakDialogLastShown";
    
    private string dominantMood = "Happy";
    private int thisMonthCount = 10;
    private DateTime selectedDate = DateTime.Today;

    private string writingPrompt = "What made you smile today?";

    private readonly List<string> writingPrompts = new()
    {
        "What made you smile today?",
        "Describe a moment you felt proud of yourself recently.",
        "What are three things you're grateful for right now?",
        "If you could relive one day from this week, which would it be and why?",
        "What's something new you learned about yourself lately?",
        "Write about a challenge you overcame and how it made you stronger.",
        "What does your ideal day look like?",
        "Who inspires you and why?",
        "What's a goal you're working towards?",
        "Describe a place where you feel most at peace.",
        "What advice would you give to your younger self?",
        "Write about something you're looking forward to.",
        "What's a recent accomplishment, big or small?",
        "How do you practice self-care?",
        "What's one thing you'd like to change about your routine?"
    };

    private void LoadNewPrompt()
    {
        var random = new Random();
        var newPrompt = writingPrompts[random.Next(writingPrompts.Count)];
    
        // Make sure we don't get the same prompt twice in a row
        while (newPrompt == writingPrompt && writingPrompts.Count > 1)
        {
            newPrompt = writingPrompts[random.Next(writingPrompts.Count)];
        }
    
        writingPrompt = newPrompt;
    }
    private string GetEntryPreview(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "No content";
    
        // Strip HTML tags if content contains HTML
        var plainText = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);
    
        // Return first 150 characters
        return plainText.Length > 150 
            ? plainText.Substring(0, 150) + "..." 
            : plainText;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine($"StreakDialogLastShown");
            await TryShowStreakDialog();
        }
    }

    private async Task TryShowStreakDialog()
    {
        // Check if already shown today
        if (!await StreaksRepository.ShouldShowStreakDialogAsync()) return;

        var streakInfo = await StreaksRepository.CreateAsync();
        int totalEntries = await StreaksRepository.GetTotalCountAsync();

        // Mark as shown today
        MarkStreakDialogShown();

        var parameters = new DialogParameters<StreakDialog>
        {
            { x => x.CurrentStreak, totalEntries },
            { x => x.TotalEntries, totalEntries },
            { x => x.LongestStreak, totalEntries },
            { x => x.AchievementName, "Daily Streak!" },
            { x => x.AchievementDescription, $"You're on a {totalEntries}-day streak!" },
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            NoHeader = true
        };

        await DialogService.ShowAsync<StreakDialog>("Your Streak!", parameters, options);
    }


    private void MarkStreakDialogShown()
    {
        Preferences.Set(StreakDialogLastShownKey, DateTime.Today.ToString("O"));
    }

    private int totalEntries = 24;
    private int currentStreak = 7;
    private int thisMonth = 15;
    private int avgWords = 342;
    private string selectedMood = string.Empty;

    private string dailyQuote = "The secret of getting ahead is getting started.";
    private string quoteAuthor = "Mark Twain";

    private List<JournalEntryPreview> recentEntries = new();

    private List<MoodOption> moods = new()
    {
    new MoodOption { 
        Label = "Great", 
        Value = "great", 
        Icon = Icons.Material.Filled.SentimentVerySatisfied, 
        Color = MudBlazor.Color.Success },
    new MoodOption
    {
        Label = "Good",
        Value = "good", 
        Icon = Icons.Material.Filled.SentimentSatisfied, 
        Color = MudBlazor.Color.Info
    },
    new MoodOption
    {
        Label = "Okay", 
        Value = "okay", 
        Icon = Icons.Material.Filled.SentimentNeutral, 
        Color = MudBlazor.Color.Warning
    },
    new MoodOption { 
        Label = "Bad",
        Value = "bad", 
        Icon = Icons.Material.Filled.SentimentDissatisfied, 
        Color = MudBlazor.Color.Error
    },
    new MoodOption {
        Label = "Terrible", 
        Value = "terrible",
        Icon = Icons.Material.Filled.SentimentVeryDissatisfied, 
        Color = MudBlazor.Color.Dark }
    };

    protected override async Task OnInitializedAsync()
    {
        // TODO: Load actual data from database
        LoadRecentEntries();
    }

    private void LoadRecentEntries()
    {
        // Sample data - replace with actual database calls
        recentEntries = new List<JournalEntryPreview>
{
            new JournalEntryPreview
            {
                Title = "A Productive Day",
                Date = DateTime.Today.AddDays(-1),
                Content = "Today was incredibly productive. I managed to complete all my tasks and even had time for a walk in the park. The weather was perfect, and I felt a great sense of accomplishment.",
                WordCount = 245,
                Mood = "great"
            },
            new JournalEntryPreview
            {
                Title = "Reflection on the Week",
                Date = DateTime.Today.AddDays(-2),
                Content = "This week has been full of challenges but also growth. I learned a lot about myself and how I handle stress. Looking forward to a relaxing weekend.",
                WordCount = 189,
                Mood = "good"
            },
            new JournalEntryPreview
            {
                Title = "Morning Thoughts",
                Date = DateTime.Today.AddDays(-3),
                Content = "Woke up feeling refreshed and ready to tackle the day. Sometimes a good night's sleep makes all the difference.",
                WordCount = 15,
                Mood = "good"
            }
        };
    }

    private string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        if (hour < 12) return "Good Morning";
        if (hour < 18) return "Good Afternoon";
        return "Good Evening";
    }

    private string GetPreview(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "No content";

        return content.Length > 120 ? content.Substring(0, 120) + "..." : content;
    }

    private string GetMoodIcon(string mood)
    {
        return mood switch
        {
            "great" => Icons.Material.Filled.SentimentVerySatisfied,
            "good" => Icons.Material.Filled.SentimentSatisfied,
            "okay" => Icons.Material.Filled.SentimentNeutral,
            "bad" => Icons.Material.Filled.SentimentDissatisfied,
            "terrible" => Icons.Material.Filled.SentimentVeryDissatisfied,
            _ => Icons.Material.Filled.SentimentNeutral
        };
    }

    private MudBlazor.Color  GetMoodColor(string mood)
    {
        return mood switch
        {
            "great" => MudBlazor.Color.Success,
            "good" => MudBlazor.Color.Info,
            "okay" => MudBlazor.Color.Warning,
            "bad" => MudBlazor.Color.Error,
            "terrible" => MudBlazor.Color.Dark,
            _ => MudBlazor.Color.Default
        };
    }

    private void SelectMood(string mood)
    {
        selectedMood = mood;
    }

    private void NavigateToEntry(DateTime date)
    {
        NavigationManager.NavigateTo($"/journal");
    }

    public class JournalEntryPreview
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public string Mood { get; set; } = string.Empty;
        public List<string> Moods { get; set; } = new(); 
    }

    public class MoodOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public MudBlazor.Color Color { get; set; }
    }
}