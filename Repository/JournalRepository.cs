using SQLite;

namespace Journal.Repository;

public class JournalRepository: IRepository<Models.Journal>
{
    private readonly SQLiteAsyncConnection _db;
    private bool _isInitialized;

    public JournalRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }
    
    private async Task InitializeAsync()
    {
        if (_isInitialized) return;
        await _db.CreateTableAsync<Models.Journal>();
        _isInitialized = true;
    }
    public async Task<List<Models.Journal>> GetAllAsync()
    {
        await InitializeAsync();
        return await _db.Table<Models.Journal>()
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }
    public async Task<Models.Journal?> GetByDateAsync(DateTime date)
    {
        await InitializeAsync();
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);
        
        return await _db.Table<Models.Journal>()
            .Where(e => e.Date >= startOfDay && e.Date < endOfDay)
            .FirstOrDefaultAsync();
    }

    public async Task<Models.Journal?> GetAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<Models.Journal>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Models.Journal entry)
    {
        await InitializeAsync();
        entry.UpdatedAt = DateTime.Now;
        
        var existing = await GetByDateAsync(entry.Date);
        if (existing != null)
        {
            throw new InvalidOperationException($"An entry already exists for {entry.Date:MMMM dd, yyyy}. Try selecting tomorrow or delete today's entry instead.");
        }
        
        return await _db.InsertAsync(entry);
    }
    public async Task<List<Models.Journal>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.Table<Models.Journal>()
            .Where(j => j.Date >= startDate && j.Date <= endDate)
            .OrderByDescending(j => j.Date)
            .ToListAsync();
    }
    
    public async Task<List<Models.Journal>> GetRecentAsync(int count = 10)
    {
        await InitializeAsync();
        return await _db.Table<Models.Journal>()
            .OrderByDescending(e => e.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> DeleteAsync(Models.Journal entry)
    {
        await InitializeAsync();
        return await _db.DeleteAsync(entry);
    }
    
    public async Task<Models.Journal?> GetByIdAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<Models.Journal>()
            .Where(j => j.Id == id)
            .FirstOrDefaultAsync();
    }
}