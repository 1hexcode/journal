using Journal.Models;
using SQLite;

namespace Journal.Repository;

public class StreaksRepository : IRepository<Streaks>
{
    private readonly SQLiteAsyncConnection _db;
    private readonly JournalRepository _journalRepository;
    private bool _isInitialized;

    public StreaksRepository(SQLiteAsyncConnection db, JournalRepository journalRepository)
    {
        _db = db;
        _journalRepository = journalRepository;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized) return;
        await _db.CreateTableAsync<Streaks>();
        _isInitialized = true;
    }

    public async Task<List<Streaks>> GetAllAsync()
    {
        await InitializeAsync();
        return await _db.Table<Streaks>().ToListAsync();
    }

    public async Task<Streaks?> GetLatestAsync()
    {
        await InitializeAsync();
        return await _db.Table<Streaks>()
            .OrderByDescending(s => s.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<Streaks?> GetAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<Streaks>()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ShouldShowStreakDialogAsync()
    {
        var streak = await GetLatestAsync();
        Console.WriteLine(
            $"{streak?.LastEntryDate.Date}: {DateTime.Today} :{streak?.LastEntryDate.Date.AddDays(1) > DateTime.Today}");
        // No streak to show
        if (streak?.LastEntryDate.Date.AddDays(1) > DateTime.Today) return false;

        return true;
    }

    public async Task<Streaks> CreateAsync()
    {
        await InitializeAsync();

        var streak = new Streaks
        {
            Id = Guid.NewGuid(),
            CurrentCount = 0,
            LongestCount = 0,
            TotalEntries = 0,
            LastEntryDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _db.InsertAsync(streak);
        return streak;
    }

    public async Task<int> GetTotalCountAsync()
    {
        await InitializeAsync();
        return await _db.Table<Streaks>().CountAsync();
    }

    public async Task<int> SaveAsync(Streaks entity)
    {
        await InitializeAsync();
        entity.UpdatedAt = DateTime.Now;

        var existing = await GetAsync(entity.Id);
        if (existing != null)
        {
            return await _db.UpdateAsync(entity);
        }

        entity.CreatedAt = DateTime.Now;
        return await _db.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(Streaks entity)
    {
        await InitializeAsync();
        return await _db.DeleteAsync(entity);
    }
    
    // === Activity Data (derived from Journal table) ===

    /// <summary>
    /// Get activity data for heatmap display
    /// </summary>
    public async Task<List<ActivityData>> GetActivityDataAsync(int months = 12)
    {
        var endDate = DateTime.Today;
        var startDate = endDate.AddMonths(-months);

        var journals = await _journalRepository.GetAllAsync();

        return journals
            .Where(j => j.Date >= startDate && j.Date <= endDate)
            .GroupBy(j => j.Date.Date)
            .Select(g => new ActivityData
            {
                Date = g.Key,
                EntryCount = g.Count()
            })
            .OrderBy(a => a.Date)
            .ToList();
    }

    /// <summary>
    /// Get activity for current month
    /// </summary>
    public async Task<List<ActivityData>> GetCurrentMonthActivityAsync()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var journals = await _journalRepository.GetAllAsync();

        return journals
            .Where(j => j.Date >= startOfMonth && j.Date <= today)
            .GroupBy(j => j.Date.Date)
            .Select(g => new ActivityData
            {
                Date = g.Key,
                EntryCount = g.Count()
            })
            .OrderBy(a => a.Date)
            .ToList();
    }

    /// <summary>
    /// Get total active days
    /// </summary>
    public async Task<int> GetTotalActiveDaysAsync()
    {
        var journals = await _journalRepository.GetAllAsync();
        return journals.Select(j => j.Date.Date).Distinct().Count();
    }
}
// === Achievements (computed on-the-fly) ===


// === Data Transfer Objects ===

public class ActivityData
{
    public DateTime Date { get; set; }
    public int EntryCount { get; set; }
    
    public int Intensity => EntryCount switch
    {
        0 => 0,
        1 => 1,
        2 => 2,
        3 => 3,
        _ => 4
    };
}

public class AchievementData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int GoalValue { get; set; }
    public int CurrentProgress { get; set; }
    public string Icon { get; set; }

    public bool IsUnlocked => CurrentProgress >= GoalValue;
    public int ProgressPercent => Math.Min(100, (int)((double)CurrentProgress / GoalValue * 100));

    public AchievementData(string name, string description, int goalValue, int currentProgress, string icon)
    {
        Name = name;
        Description = description;
        GoalValue = goalValue;
        CurrentProgress = Math.Min(currentProgress, goalValue);
        Icon = icon;
    }
}