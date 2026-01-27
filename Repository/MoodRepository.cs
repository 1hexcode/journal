using Journal.Models;
using SQLite;

namespace Journal.Repository;

public class MoodRepository : IRepository<Mood>
{
    private readonly SQLiteAsyncConnection _db;
    private bool _isInitialized;
    
    public MoodRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized) return;

        await _db.CreateTableAsync<Mood>();
        await SeedMoodsAsync();
        
        _isInitialized = true;
    }

    /// <summary>
    /// Public method to initialize the repository and seed moods data.
    /// This can be called explicitly at application startup to ensure data is seeded.
    /// </summary>
    public async Task InitializeAndSeedAsync()
    {
        await InitializeAsync();
    }

    public async Task SeedMoodsAsync()
    {
        
        var count = await _db.Table<Mood>().CountAsync();
        if (count > 0) return;
        Console.WriteLine($"Mood Seeder Initialized");
        var moods = new List<Mood>();

        // Adding your required data
        moods.AddRange(CreateGroup("Positive", "Happy", "Excited", "Relaxed", "Grateful", "Confident"));
        moods.AddRange(CreateGroup("Neutral", "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored"));
        moods.AddRange(CreateGroup("Negative", "Sad", "Angry", "Stressed", "Lonely", "Anxious"));

        await _db.InsertAllAsync(moods);
    }

    private List<Mood> CreateGroup(string category, params string[] names)
    {
        return names.Select(name => new Mood
        {
            Id = Guid.NewGuid(),
            MoodName = name,
            MoodCategory = category,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();
    }

    // --- Interface Methods ---

    public async Task<List<Mood>> GetAllAsync()
    {
        await InitializeAsync();
        return await _db.Table<Mood>().ToListAsync();
    }

    public async Task<Mood> GetAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<Mood>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Mood entity)
    {
        await InitializeAsync();
        var exists = await GetAsync(entity.Id);
        if (exists != null)
        {
            return await _db.UpdateAsync(entity);
        }
        return await _db.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(Mood entity)
    {
        await InitializeAsync();
        return await _db.DeleteAsync(entity);
    }
    
    public async Task<List<Mood>> GetByCategoryAsync(string category)
    {
        await InitializeAsync();
        return await _db.Table<Mood>()
            .Where(m => m.MoodCategory == category)
            .ToListAsync();
    }
}