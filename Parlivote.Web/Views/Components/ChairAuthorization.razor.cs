using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Components;

public partial class ChairAuthorization : ComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}