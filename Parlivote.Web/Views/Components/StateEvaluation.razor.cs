using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;

namespace Parlivote.Web.Views.Components;

public partial class StateEvaluation : ComponentBase
{
    [Parameter]
    public ComponentState State { get; set; }

    [Parameter]
    public RenderFragment Loading { get; set; } = null;

    [Parameter]
    public string LoadingText { get; set; }

    [Parameter]
    public RenderFragment Content { get; set; }

    [Parameter]
    public RenderFragment Error { get; set; } = null;

    [Parameter]
    public string ErrorMessage { get; set; } = null;
}