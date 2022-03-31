using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class ListGroup : ComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}