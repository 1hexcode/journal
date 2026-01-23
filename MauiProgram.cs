using Microsoft.Extensions.Logging;

using MudBlazor.Services;

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

        builder.Services.AddMudServices();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        return builder.Build();
    }
}