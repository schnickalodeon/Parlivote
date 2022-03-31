using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class FormControlBase : ComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public string LabelText { get; set; }
}