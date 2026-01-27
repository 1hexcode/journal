using MudBlazor;
using Microsoft.AspNetCore.Components.Web;


namespace Journal.Components.Pages;

public partial class Search
{
     private string searchQuery = "";
    private DateRange? dateRange = null;
    private string selectedMood = "";
    private string wordCountFilter = "all";
    private string sortBy = "date-desc";
    private string viewMode = "grid";
    
    private bool isSearching = false;
    private bool hasSearched = false;
    private List<SearchResult> searchResults = new();
    private List<string> recentSearches = new() { "gratitude", "work stress", "family time" };

    protected override void OnInitialized()
    {
        if (!AuthService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await PerformSearch();
        }
    }

    private async Task PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(searchQuery) && dateRange == null && string.IsNullOrEmpty(selectedMood))
        {
            Snackbar.Add("Please enter a search term or select filters", Severity.Warning);
            return;
        }

        isSearching = true;
        hasSearched = false;
        StateHasChanged();

        // Simulate search delay
        await Task.Delay(500);

        // TODO: Replace with actual search logic
        searchResults = GetMockSearchResults();

        // Add to recent searches
        if (!string.IsNullOrWhiteSpace(searchQuery) && !recentSearches.Contains(searchQuery))
        {
            recentSearches.Insert(0, searchQuery);
            if (recentSearches.Count > 5) recentSearches.RemoveAt(5);
        }

        isSearching = false;
        hasSearched = true;
        StateHasChanged();
    }

    private void ClearFilters()
    {
        searchQuery = "";
        dateRange = null;
        selectedMood = "";
        wordCountFilter = "all";
        sortBy = "date-desc";
        hasSearched = false;
        searchResults.Clear();
    }

    private void ClearHistory()
    {
        recentSearches.Clear();
        Snackbar.Add("Search history cleared", Severity.Info);
    }

    private async Task QuickFilter(string filter)
    {
        switch (filter)
        {
            case "today":
                dateRange = new DateRange(DateTime.Today, DateTime.Today);
                break;
            case "week":
                dateRange = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
                break;
            case "month":
                dateRange = new DateRange(DateTime.Today.AddMonths(-1), DateTime.Today);
                break;
        }
        await PerformSearch();
    }

    private async Task SearchByTag(string tag)
    {
        searchQuery = tag;
        await PerformSearch();
    }

    private string HighlightText(string text)
    {
        // Simple highlight implementation
        // TODO: Implement proper HTML highlighting
        return text.Length > 200 ? text.Substring(0, 200) + "..." : text;
    }

    private MudBlazor.Color GetMoodColor(string mood)
    {
        if (mood.Contains("Happy") || mood.Contains("Grateful")) return MudBlazor.Color.Success;
        if (mood.Contains("Sad") || mood.Contains("Frustrated")) return MudBlazor.Color.Error;
        if (mood.Contains("Peaceful")) return MudBlazor.Color.Info;
        return MudBlazor.Color.Default;
    }

    private List<TagData> GetPopularTags()
    {
        return new List<TagData>
        {
            new("gratitude", 24), new("work", 18), new("family", 15),
            new("health", 12), new("goals", 10), new("reflection", 8)
        };
    }

    private List<SearchResult> GetMockSearchResults()
    {
        // TODO: Replace with actual database search
        var random = new Random();
        return Enumerable.Range(1, 8).Select(i => new SearchResult
        {
            Date = DateTime.Today.AddDays(-random.Next(1, 90)),
            Content = "Today was an interesting day filled with many thoughts and reflections. I spent time contemplating my goals and the progress I've made so far this year. There were challenges, but also moments of joy and gratitude.",
            WordCount = random.Next(150, 800),
            Mood = new[] { "😊 Happy", "😌 Peaceful", "🤗 Grateful" }[random.Next(3)],
            Tags = new List<string> { "gratitude", "reflection", "goals" }
        }).ToList();
    }

    private void ViewEntry(SearchResult entry)
    {
        Snackbar.Add($"Viewing entry from {entry.Date:MMM dd}", Severity.Info);
        // TODO: Navigate to entry detail page
    }

    private void EditEntry(SearchResult entry)
    {
        NavigationManager.NavigateTo($"/journal?date={entry.Date:yyyy-MM-dd}");
    }

    public class SearchResult
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int WordCount { get; set; }
        public string Mood { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    record TagData(string Name, int Count);
}