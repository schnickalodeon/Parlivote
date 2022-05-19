using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingAttendanceComponent : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

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

    private async void UpdateAttendance(bool attendance)
    {
        Func<MeetingView, Guid, Task<MeetingView>> attendanceFunction = null;
        if (attendance)
        {
            attendanceFunction = this.MeetingViewService.AddAttendance;
        }
        else
        {
            attendanceFunction = this.MeetingViewService.RemoveAttendance;
        }

        Guid userId = await GetUserId();
        await attendanceFunction(this.MeetingView, userId);
    }

    private async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string userId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(userId);
    }
}