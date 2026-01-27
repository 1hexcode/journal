using Journal.Models;
using SQLite;

namespace Journal.Repository;

public class UserRepository : IRepository<User>
{
    private readonly SQLiteAsyncConnection _db;
    private bool _isInitialized;

    public UserRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized) return;

        await _db.CreateTableAsync<User>();
        await SeedUserAsync();
        
        _isInitialized = true;
    }

    public async Task InitializeAndSeedAsync()
    {
        await InitializeAsync();
    }

    private async Task SeedUserAsync()
    {
        
        // Check if a user already exists
        var count = await _db.Table<User>().CountAsync();
        if (count > 0) return;
        
        Console.WriteLine($"User Seeder Initialized");
        // Create a default local profile
        var defaultUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "journal_user",
            FirstName = "Journal",
            LastName = "Owner",
            Email = "user@email.com",
            Password = "login", // Usually empty if using biometric or no-auth local storage
            HasPassword = false, // Setting to false as a default for local-first apps
            CreatedAt = DateTime.Now
        };

        await _db.InsertAsync(defaultUser);
    }

    // --- IRepository Implementation ---

    public async Task<List<User>> GetAllAsync()
    {
        await InitializeAsync();
        return await _db.Table<User>().ToListAsync();
    }

    public async Task<User> GetAsync(Guid id)
    {
        await InitializeAsync();
        return await _db.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(User entity)
    {
        await InitializeAsync();
        var exists = await GetAsync(entity.Id);
        if (exists != null)
        {
            return await _db.UpdateAsync(entity);
        }
        return await _db.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(User entity)
    {
        await InitializeAsync();
        return await _db.DeleteAsync(entity);
    }

    /// <summary>
    /// Helper to get the primary user of the app
    /// </summary>
    public async Task<User> GetDefaultUserAsync()
    {
        await InitializeAsync();
        return await _db.Table<User>().FirstOrDefaultAsync();
    }
}