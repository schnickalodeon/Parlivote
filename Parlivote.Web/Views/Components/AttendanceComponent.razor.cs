using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components;

public partial class AttendanceComponent : ComponentBase
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private HubConnection hubConnection;
    private bool IsConnected => 
        this.hubConnection.State == HubConnectionState.Connected;

    private Guid userId;

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

    protected override async Task OnInitializedAsync()
    {
        this.userId = await GetUserId();
        this.isAttendant = 
        await ConnectToVoteHub();
    }

    private async Task ConnectToVoteHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/votehub"))
            .Build();

        await this.hubConnection.StartAsync();
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

        MeetingView updatedMeetingView = 
            await attendanceFunction(this.MeetingView, this.userId);

        if (IsConnected)
        {
            await this.hubConnection.InvokeAsync(VoteHub.AttendanceUpdatedMethod, updatedMeetingView);
        }
        else
        {
            throw new Exception("Not Connected");
        }
        
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