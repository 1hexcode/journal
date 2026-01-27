using Journal.Models;
using SQLite;

namespace Journal.Data;

public class DatabaseService
{
    
    public SQLiteAsyncConnection Connection { get; private set; }

    public void AppDatabase()
    {
        string databasePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Journal.db"
        );
        Console.WriteLine($"Database path: {databasePath}");

        Connection = new SQLiteAsyncConnection(databasePath);

         Connection.CreateTableAsync<User>().Wait();
         Connection.CreateTableAsync<Models.Journal>().Wait();
    }
}
