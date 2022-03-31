using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingBox : ComponentBase
{
    [Parameter]
    public MeetingView Meeting { get; set; }
}