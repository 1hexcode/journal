using Journal.Dialogs;
using Journal.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components.Web;

namespace Journal.Components.Pages;

public partial class Search
{
    // === Search fields ===
    private string searchQuery = "";
    private DateRange? dateRange;
    private string selectedMood = "";
    private string wordCountFilter = "all";
    private string sortBy = "date-desc";
    private string viewMode = "grid";
    private bool isSearching;
    private bool hasSearched;
    private List<SearchResult> searchResults = [];
    private List<string> recentSearches = ["gratitude", "work stress", "family time"];

    // === Pagination fields ===
    private List<Models.Journal> allJournals = [];
    private bool isLoadingJournals;
    private int currentPage = 1;
    private int pageSize = 10;
    private int totalJournals;
    private int totalPages => (int)Math.Ceiling((double)totalJournals / pageSize);

    private IEnumerable<Models.Journal> PagedJournals => allJournals
        .OrderByDescending(j => j.Date)
        .Skip((currentPage - 1) * pageSize)
        .Take(pageSize);

    // === Lifecycle ===

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        await LoadAllJournals();
    }

    // === Journal Loading ===

    private async Task LoadAllJournals()
    {
        isLoadingJournals = true;
        StateHasChanged();

        try
        {
            allJournals = await JournalRepository.GetAllAsync();
            totalJournals = allJournals.Count;
        }
        finally
        {
            isLoadingJournals = false;
            StateHasChanged();
        }
    }

    private void OnPageChanged(int page)
    {
        currentPage = page;
        StateHasChanged();
    }

    // === Search Methods ===

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

        await Task.Delay(300); // Brief delay for UX

        // Actual search logic
        searchResults = await SearchJournals();

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

    private async Task<List<SearchResult>> SearchJournals()
    {
        var journals = await JournalRepository.GetAllAsync();
        var query = journals.AsEnumerable();

        // Filter by search query
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var term = searchQuery.ToLower();
            query = query.Where(j =>
                (j.Notes?.ToLower().Contains(term) ?? false) ||
                (j.PrimaryMood?.ToLower().Contains(term) ?? false) ||
                (j.SecondaryMoods?.ToLower().Contains(term) ?? false) ||
                (j.PredefinedTags?.ToLower().Contains(term) ?? false) ||
                (j.CustomTags?.ToLower().Contains(term) ?? false));
        }

        // Filter by date range
        if (dateRange?.Start != null && dateRange?.End != null)
        {
            query = query.Where(j => j.Date >= dateRange.Start && j.Date <= dateRange.End);
        }

        // Filter by mood
        if (!string.IsNullOrEmpty(selectedMood))
        {
            var mood = selectedMood.ToLower();
            query = query.Where(j =>
                (j.PrimaryMood?.ToLower().Contains(mood) ?? false) ||
                (j.SecondaryMoods?.ToLower().Contains(mood) ?? false));
        }

        // Sort
        query = sortBy switch
        {
            "date-asc" => query.OrderBy(j => j.Date),
            "date-desc" => query.OrderByDescending(j => j.Date),
            "word-count" => query.OrderByDescending(j => GetWordCount(j.Notes)),
            _ => query.OrderByDescending(j => j.Date)
        };

        return query.Select(j => new SearchResult
        {
            Id = j.Id,
            Date = j.Date,
            Content = j.Notes ?? "",
            WordCount = GetWordCount(j.Notes),
            Mood = j.PrimaryMood ?? "",
            Tags = GetTagsList(j)
        }).ToList();
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
        dateRange = filter switch
        {
            "today" => new DateRange(DateTime.Today, DateTime.Today),
            "week" => new DateRange(DateTime.Today.AddDays(-7), DateTime.Today),
            "month" => new DateRange(DateTime.Today.AddMonths(-1), DateTime.Today),
            "favorites" => null, // TODO: Implement favorites filter
            _ => null
        };
        await PerformSearch();
    }

    private async Task SearchByTag(string tag)
    {
        searchQuery = tag;
        await PerformSearch();
    }

    // === Action Methods ===

    private void ViewEntry(SearchResult entry)
    {
        NavigationManager.NavigateTo($"/journal/view/{entry.Id}");
    }

    private void EditEntry(SearchResult entry)
    {
        NavigationManager.NavigateTo($"/journal/edit/{entry.Id}");
    }

    private void ViewJournal(Models.Journal journal)
    {
        NavigationManager.NavigateTo($"/journal/view/{journal.Id}");
    }

    private void EditJournal(Models.Journal journal)
    {
        NavigationManager.NavigateTo($"/journal/edit/{journal.Id}");
    }

    private async Task DeleteJournal(Models.Journal journal)
    {
        var result = await DialogService.ShowMessageBox(
            "Delete Entry",
            $"Are you sure you want to delete the entry from {journal.Date:MMM dd, yyyy}?",
            yesText: "Delete",
            cancelText: "Cancel");

        if (result == true)
        {
            await JournalRepository.DeleteAsync(journal);
            Snackbar.Add("Entry deleted", Severity.Success);
            await LoadAllJournals();
        }
    }

    // === Helper Methods ===

    private MudBlazor.Color GetMoodColor(string? mood)
    {
        if (string.IsNullOrEmpty(mood)) return MudBlazor.Color.Default;

        return mood.ToLower() switch
        {
            var m when m.Contains("happy") || m.Contains("grateful") || m.Contains("positive") => MudBlazor.Color.Success,
            var m when m.Contains("sad") || m.Contains("frustrated") || m.Contains("negative") => MudBlazor.Color.Error,
            var m when m.Contains("peaceful") || m.Contains("calm") || m.Contains("neutral") => MudBlazor.Color.Info,
            _ => MudBlazor.Color.Default
        };
    }

    private string HighlightText(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        var plain = GetPlainText(text);
        return plain.Length > 200 ? plain[..200] + "..." : plain;
    }

    private string GetPlainText(string? html)
    {
        if (string.IsNullOrEmpty(html)) return "";
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
        text = System.Net.WebUtility.HtmlDecode(text);
        return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
    }

    private string GetPlainTextPreview(string? html, int maxLength)
    {
        var text = GetPlainText(html);
        return text.Length > maxLength ? text[..maxLength] + "..." : text;
    }

    private int GetWordCount(string? text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        var plain = GetPlainText(text);
        return plain.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private List<string> GetTagsList(Models.Journal journal)
    {
        var tags = new List<string>();

        if (!string.IsNullOrEmpty(journal.PredefinedTags))
            tags.AddRange(journal.PredefinedTags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));

        if (!string.IsNullOrEmpty(journal.CustomTags))
            tags.AddRange(journal.CustomTags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));

        return tags;
    }

    private List<TagData> GetPopularTags()
    {
        // Calculate from actual data
        var tagCounts = allJournals
            .SelectMany(j => GetTagsList(j))
            .GroupBy(t => t.ToLower())
            .Select(g => new TagData(g.Key, g.Count()))
            .OrderByDescending(t => t.Count)
            .Take(6)
            .ToList();

        return tagCounts.Any() ? tagCounts : 
        [
            new("gratitude", 0),
            new("work", 0),
            new("family", 0)
        ];
    }

    // === Models ===

    public class SearchResult
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; } = "";
        public int WordCount { get; set; }
        public string Mood { get; set; } = "";
        public List<string> Tags { get; set; } = [];
    }

    public record TagData(string Name, int Count);
    
    
    // Export
    private DateTime? exportStartDate = null;
    private DateTime? exportEndDate = null;

    private void SetExportRange(string range)
    {
        exportEndDate = DateTime.Today;
    
        exportStartDate = range switch
        {
            "week" => DateTime.Today.AddDays(-7),
            "month" => DateTime.Today.AddMonths(-1),
            "year" => new DateTime(DateTime.Today.Year, 1, 1),
            _ => DateTime.Today
        };
    
        StateHasChanged();
    }
    
    
    public async Task ExportToPdf()
    {
        if (!exportStartDate.HasValue || !exportEndDate.HasValue)
        {
            Snackbar.Add("Please select date range", Severity.Warning);
            return;
        }

        try
        {
            Console.WriteLine("Starting export...");
        
            var allJournals = await JournalRepository.GetByDateRangeAsync(
                exportStartDate.Value, 
                exportEndDate.Value
            );
        
            Console.WriteLine($"Found {allJournals.Count()} journals");
        
            if (!allJournals.Any())
            {
                Snackbar.Add("No entries found in selected date range", Severity.Warning);
                return;
            }

            var content = PdfExportService.ExportJournalTable(allJournals);
            Console.WriteLine($"Content length: {content.Length}");
        
            var fileName = $"journal_{exportStartDate:yyyy-MM-dd}_to_{exportEndDate:yyyy-MM-dd}.pdf";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        
            await File.WriteAllTextAsync(filePath, content);
            Console.WriteLine($"File written to: {filePath}");
        
            Snackbar.Add($"Exported {allJournals.Count()} entries!", Severity.Success);
            Snackbar.Add($"Saved to: {filePath}", Severity.Info);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export failed: {ex}");
            Snackbar.Add($"Export failed: {ex.Message}", Severity.Error);
        }
    }

}