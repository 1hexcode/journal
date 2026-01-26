using Journal.Components.Pages;
using Journal.Dialogs;
using MudBlazor;

namespace Journal.Components.Buttons;

public partial class LogoutButton
{
    private async Task Logout()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to Logout?" },
            { "ButtonText", "Logout" },
            { "Color", MudBlazor.Color.Error }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true };
    
        IDialogReference dialog = await DialogService.ShowAsync<ConfirmationDialog>(
            "Logout", 
            parameters, 
            options
        );
    
        DialogResult result = await dialog.Result;

        if (!result.Canceled)
        {
            // TODO: AuthService.LogOut();
            Snackbar.Clear();
            Snackbar.Add("Logged out!", Severity.Success);
            NavigationManager.NavigateTo(Login.Route, replace: true);
        }
    }
}