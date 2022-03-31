using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class ListGroupItem : ComponentBase
{
    [Parameter]
    public RenderFragment Title { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter] 
    public RenderFragment Additional { get; set; }
}