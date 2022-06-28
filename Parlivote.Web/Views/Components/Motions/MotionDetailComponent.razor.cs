using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Motions;

public partial class MotionDetailComponent : ComponentBase
{
    [Parameter]
    public MotionView Motion { get; set; }

    private InfoDialogBase infoDialogBase;

    public void Show()
    {
        this.infoDialogBase.Show();
    }
}