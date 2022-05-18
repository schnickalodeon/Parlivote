using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class CheckBoxBase : ComponentBase
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public bool IsChecked { get; set; }
}