using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingAttendanceComponent
{
    [Inject]
    IMeetingViewService meetingViewService;

    [Parameter]
    public MeetingView MeetingView { get; set; }

    private bool isAttendant;
    public bool IsAttendant
    {
        get => this.isAttendant;
        set
        {
            this.isAttendant = value;
            UpdateAttendance(value);
        }
    }

    private static void UpdateAttendance(bool attendance)
    {
        
    }
}