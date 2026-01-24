using MudBlazor;

namespace Journal.Components.Pages;

using Journal.Dialogs;

public partial class Home
{
    public const string Route = "/home";

    // In-memory list of journal dates for streak calculation
    private List<DateTime> UserEntries = new()
    {
        DateTime.Today.AddDays(-2),
        DateTime.Today.AddDays(-1),
        DateTime.Today // example streak: 3 days
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Ensure UI is rendered before opening dialog
            await InvokeAsync(async () =>
            {
                await Task.Delay(150); // small delay
                ShowStreakDialog();
            });
        }
    }

    private void ShowStreakDialog()
    {
        int streak = CalculateStreak();

        if (streak == 0) return;

        var parameters = new DialogParameters
        {
            ["StreakDays"] = streak
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        DialogService.Show<StreakDialog>("Your Streak!", parameters, options);
    }

    private int CalculateStreak()
    {
        if (!UserEntries.Any()) return 0;

        var ordered = UserEntries
            .OrderByDescending(d => d.Date)
            .Select(d => d.Date)
            .ToList();

        int streak = 0;
        DateTime today = DateTime.Today;

        foreach (var date in ordered)
        {
            if (date == today.AddDays(-streak))
                streak++;
            else
                break;
        }

        return streak;
    }
}