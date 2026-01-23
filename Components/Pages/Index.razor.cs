namespace Journal.Components.Pages;

public partial class Index
{
    public const string Route = "/";

    protected sealed override async Task OnInitializedAsync()
    {
        try
        {
            await Task.Delay(1000);
            NavigationManager.NavigateTo(Login.Route);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
        await Task.Delay(1000);
    }
}