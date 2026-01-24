using SQLite;

namespace Journal.Data;

public class DatabaseService
{
    
    public SQLiteAsyncConnection Connection { get; private set; }

    public void AppDatabase()
    {
        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "Journal.db"
        );

        Connection = new SQLiteAsyncConnection(databasePath);

        // Connection.CreateTableAsync<Product>().Wait();
        // Connection.CreateTableAsync<Customer>().Wait();
        // create other tables in a similar way.
    }
}
