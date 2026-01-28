namespace Journal.Components.Pages;

public partial class Streaks
{
    public const string Route = "/streaks";

    private int totalEntries = 0;
    private int longestStreak = 0;
    private int currentStreak = 0;
    
    private async Task  LoadStreakData()
    {
        totalEntries = await StreaksRepository.GetTotalCountAsync();
        currentStreak = await StreaksRepository.GetTotalCountAsync();
        longestStreak = await StreaksRepository.GetTotalCountAsync();

        // Mock writing dates (last 90 days with some gaps)
        var random = new Random(42); // Fixed seed for consistent demo
        for (int i = 0; i < 90; i++)
        {
            var date = DateTime.Today.AddDays(-i);
            if (random.Next(0, 3) != 0) // 66% chance of writing
            {
                writingDates.Add(date.Date);
            }
        }
    }
}