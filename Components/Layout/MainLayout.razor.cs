using Microsoft.AspNetCore.Components;

namespace Journal.Components.Layout;

public partial class MainLayout
{
    private string _title;

    private bool _drawerOpen = true;

    [CascadingParameter]
    public ParentLayout ParentLayout { get; set; } = default!;

    // Access IsDarkMode from ParentLayout
    private bool IsDarkMode => ParentLayout?.IsDarkMode ?? false;
    
    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void SetAppBarTitle(string title)
    {
        _title = title;
    }
}