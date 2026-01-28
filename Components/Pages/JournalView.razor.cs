using Microsoft.AspNetCore.Components;
using MudBlazor;
using Journal.Repository;

namespace Journal.Components.Pages;

public partial class JournalView
{
    [Parameter] public Guid Id { get; set; }
    
    private Models.Journal? journal;
    private Models.Journal? previousJournal;
    private Models.Journal? nextJournal;
    private bool isLoading = true;

    protected override async Task OnParametersSetAsync()
    {
        await LoadJournal();
    }

    private async Task LoadJournal()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            journal = await JournalRepository.GetByIdAsync(Id);

            if (journal != null)
            {
                await LoadAdjacentJournals();
            }
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadAdjacentJournals()
    {
        if (journal == null) return;

        var allJournals = await JournalRepository.GetAllAsync();
        var ordered = allJournals.OrderByDescending(j => j.Date).ToList();
        var currentIndex = ordered.FindIndex(j => j.Id == journal.Id);

        previousJournal = currentIndex < ordered.Count - 1 ? ordered[currentIndex + 1] : null;
        nextJournal = currentIndex > 0 ? ordered[currentIndex - 1] : null;
    }

    // === Helper Methods ===

    private string GetDaysAgo()
    {
        if (journal == null) return "";

        var days = (DateTime.Today - journal.Date.Date).Days;
        return days switch
        {
            0 => "Today",
            1 => "Yesterday",
            < 7 => $"{days} days ago",
            < 30 => $"{days / 7} week{(days / 7 > 1 ? "s" : "")} ago",
            _ => $"{days / 30} month{(days / 30 > 1 ? "s" : "")} ago"
        };
    }

    private List<string> GetSecondaryMoodsList()
    {
        if (string.IsNullOrEmpty(journal?.SecondaryMoods)) return [];
        return journal.SecondaryMoods
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(m => m.Trim())
            .ToList();
    }

    private List<string> GetAllTags()
    {
        var tags = new List<string>();

        if (!string.IsNullOrEmpty(journal?.PredefinedTags))
            tags.AddRange(journal.PredefinedTags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));

        if (!string.IsNullOrEmpty(journal?.CustomTags))
            tags.AddRange(journal.CustomTags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));

        return tags;
    }

    private MudBlazor.Color GetMoodColor(string? mood)
    {
        if (string.IsNullOrEmpty(mood)) return MudBlazor.Color.Default;

        return mood.ToLower() switch
        {
            var m when m.Contains("happy") || m.Contains("grateful") || m.Contains("positive") || m.Contains("excited") => MudBlazor.Color.Success,
            var m when m.Contains("sad") || m.Contains("frustrated") || m.Contains("negative") || m.Contains("angry") || m.Contains("anxious") => MudBlazor.Color.Error,
            var m when m.Contains("peaceful") || m.Contains("calm") || m.Contains("neutral") || m.Contains("relaxed") => MudBlazor.Color.Info,
            _ => MudBlazor.Color.Default
        };
    }

    // === Navigation ===

    private void GoBack()
    {
        NavigationManager.NavigateTo("/search");
    }

    private void EditJournal()
    {
        NavigationManager.NavigateTo($"/journal/edit/{Id}");
    }

    private void NavigateToJournal(Guid id)
    {
        NavigationManager.NavigateTo($"/journal/view/{id}");
    }

    private async Task DeleteJournal()
    {
        var result = await DialogService.ShowMessageBox(
            "Delete Entry",
            $"Are you sure you want to delete this journal entry from {journal?.Date:MMM dd, yyyy}? This action cannot be undone.",
            yesText: "Delete",
            cancelText: "Cancel");

        if (result == true && journal != null)
        {
            await JournalRepository.DeleteAsync(journal);
            Snackbar.Add("Entry deleted", Severity.Success);
            NavigationManager.NavigateTo("/search");
        }
    }
}