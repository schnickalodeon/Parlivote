using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Motions;
using Syncfusion.Blazor.Diagrams;

namespace Parlivote.Web.Views.Components.Motions;

public partial class MotionListItem : ComponentBase
{
    [Parameter]
    public MotionView Motion { get; set; }

    private string statusPillCss = "";

    protected override void OnParametersSet()
    {
        this.statusPillCss = GetPillCssByStatus();
    }

    private string GetPillCssByStatus()
    {
        return MotionStateConverter.FromString(Motion.State) switch
        {
            MotionState.Submitted => "bg-primary",
            MotionState.Pending => "bg-warning",
            MotionState.Accepted => "bg-success",
            MotionState.Declined => "bg-danger",
            _ => ""
        };
    }
}