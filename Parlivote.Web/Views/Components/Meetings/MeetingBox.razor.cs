using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Views.Base;
using Parlivote.Web.Views.Components.Motions;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingBox : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter] 
    public EventCallback OnMeetingChanged { get; set; }

    [Parameter]
    public MeetingView Meeting { get; set; }

    private ConfirmationDialog confirmationDialog;
    private AddMotionComponent addMotionComponent;
    private EditMeetingComponent editMeetingComponent;

    private HubConnection hubConnection;
    private HubConnection motionHubConnection;
    private bool IsConnected =>
        this.hubConnection.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToVoteHub();
    }

    private async Task ConnectToVoteHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/votehub"))
            .Build();

        this.hubConnection.On<MeetingView>(VoteHub.AttendanceUpdatedMethod, async (meetingView) =>
        {
            if (this.Meeting.Id == meetingView.Id)
            {
                this.Meeting = meetingView;
                await InvokeAsync(StateHasChanged);
            }
        });

        await this.hubConnection.StartAsync();
    }

    private async void DeleteMeeting()
    {
        try
        {
            await this.MeetingViewService.DeleteByIdAsync(Meeting.Id.Value);
            await OnMeetingChanged.InvokeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}