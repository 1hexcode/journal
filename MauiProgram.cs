using Journal.Data;
using Journal.Services;
using Journal.Repository;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using SQLite;

namespace Journal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });
        // Source - https://stackoverflow.com/a
        // Posted by PBo
        // Retrieved 2025-12-20, License - CC BY-SA 4.0

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
            config.SnackbarConfiguration.HideTransitionDuration = 200;
            config.SnackbarConfiguration.ShowTransitionDuration = 200;
            config.SnackbarConfiguration.MaxDisplayedSnackbars = 6;
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomStart;
        });
        
        builder.Services.AddMauiBlazorWebView();
        
        // Register SQLite connection
        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            return new SQLiteAsyncConnection(dbPath);
        });
        
        // Register repositories
        builder.Services.AddSingleton<MoodRepository>();
        builder.Services.AddSingleton<TagRepository>();  
        builder.Services.AddSingleton<UserRepository>();
        builder.Services.AddSingleton<JournalRepository>();

        
        // Register services
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddScoped<DatabaseSeeder>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        var app = builder.Build();

        // Initialize database and seed data
        Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await dbInitializer.InitializeAsync();
        }).GetAwaiter().GetResult();

        return app;
    }
}