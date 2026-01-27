using Journal.Models;
using SQLite;

namespace Journal.Repository;

public class TagRepository : IRepository<Tags> 
{
    private readonly SQLiteAsyncConnection _db;
    private bool _isInitialized;
    
    public TagRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized) return;

        await _db.CreateTableAsync<Tags>();
        await SeedTagsAsync();
        
        _isInitialized = true;
    }

    public async Task InitializeAndSeedAsync()
    {
        await InitializeAsync();
    }

    private async Task SeedTagsAsync()
    {
        var count = await _db.Table<Tags>().CountAsync();
        if (count > 0) return;
        
        Console.WriteLine($"Tags Seeder Initialized");
        var preBuiltTags = new List<string> 
        { 
            "Work", "Career", "Studies", "Family", "Friends", "Relationships",
            "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies", 
            "Travel", "Nature", "Finance", "Spirituality", "Birthday", "Holiday", 
            "Vacation", "Celebration", "Exercise", "Reading", "Writing", 
            "Cooking", "Meditation", "Yoga", "Music", "Shopping", "Parenting", 
            "Projects", "Planning", "Reflection" 
        };

        var tags = preBuiltTags.Select(name => new Tags
        {
            Id = Guid.NewGuid(),
            TagName = name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

        await _db.InsertAllAsync(tags);
    }

    // --- IRepository Methods ---

    public async Task<List<Tags>> GetAllAsync()
    {
        await InitializeAsync();
        return await _db.Table<Tags>().ToListAsync();
    }

    public async Task<Tags> GetAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<Tags>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Tags entity)
    {
        await InitializeAsync();
        // Check by Name or ID to prevent duplicates for custom tags
        var exists = await _db.Table<Tags>()
                              .Where(t => t.Id == entity.Id || t.TagName == entity.TagName)
                              .FirstOrDefaultAsync();
        if (exists != null)
        {
            return await _db.UpdateAsync(entity);
        }
        return await _db.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(Tags entity)
    {
        await InitializeAsync();
        
        return await _db.DeleteAsync(entity);
    }
}