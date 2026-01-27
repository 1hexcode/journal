using Journal.Models;
using SQLite;

namespace Journal.Services;

public class UserService
{
    private readonly SQLiteAsyncConnection _database;

    public UserService()
    {
        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "Journal.db"
        );
        Console.WriteLine($"DB Path: {databasePath}");
        _database = new SQLiteAsyncConnection(databasePath);
        _database.CreateTableAsync<User>().Wait();
    }

    // Create User
    public async Task<bool> RegisterAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.Now;
        await _database.InsertAsync(user);
        return true;
    }

    // Update User
    public async Task<int> UpdateUser(User user)
    {
        return await _database.UpdateAsync(user);
    }

    // Get User by Id
    public async Task<User> GetUser(Guid id)
    {
        return await _database.Table<User>()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    // Get User by Username
    public async Task<User> GetUserByUsername(string username)
    {
        return await _database.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
    }

    // Get User by Useremail
    public async Task<User> GetUserByUserEmail(string email)
    {
        return await _database.Table<User>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    
    // Get All Users
    public async Task<List<User>> GetAllUsers()
    {
        return await _database.Table<User>().ToListAsync();
    }

    // Delete User
    public async Task<int> DeleteUser(Guid id)
    {
        return await _database.DeleteAsync<User>(id);
    }
}