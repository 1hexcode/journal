using Journal.Components.Layout;
using Microsoft.AspNetCore.Components;

namespace Journal.Components.Buttons;

public partial class DarkModeButton
{
    [CascadingParameter]
    protected ParentLayout ParentLayout { get; set; }
}