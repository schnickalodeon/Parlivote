using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Votes;

namespace Parlivote.Web.Views.Components.Motions;

public partial class VoteComponent: ComponentBase
{
    [Inject]
    public IVoteViewService VoteViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public EventCallback AfterVoted { get; set; }

    [Parameter]
    public MotionView ActiveMotion { get; set; }

    private int attendanceCount = 0;
    private VoteValue selectedVoteValue = VoteValue.NoValue;
    private HubConnection hubConnection;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToVoteHub();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        this.attendanceCount = ActiveMotion.MeetingAttendanceCount;
    }

    private async Task ConnectToVoteHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/voteHub"))
            .Build();

        this.hubConnection.On<MeetingView>(VoteHub.AttendanceUpdatedMethod, (meetingView) =>
        {
            if (ActiveMotion.MeetingId == meetingView.Id)
            {
                this.attendanceCount = meetingView.Attendances.Count;
                InvokeAsync(StateHasChanged);
            }
        });

        this.hubConnection.On(VoteHub.VoteUpdatedMethod, async () =>
        {
            await this.AfterVoted.InvokeAsync();
            await InvokeAsync(StateHasChanged);
        });

        await this.hubConnection.StartAsync();
    }

    private void ForClicked()
    {
        this.selectedVoteValue = VoteValue.For;
        StateHasChanged();
    }

    private void AgainstClicked()
    {
        this.selectedVoteValue = VoteValue.Against;
        StateHasChanged();
    }

    private void AbstentionClicked()
    {
        this.selectedVoteValue = VoteValue.Abstention;
        StateHasChanged();
    }

    private string GetSubmitButtonCss()
    {
        return this.selectedVoteValue switch
        {
            VoteValue.For => "btn btn-success",
            VoteValue.Against => "btn btn-danger",
            VoteValue.Abstention => "btn btn-warning",
            _ => "invisible"
        };
    }

    private async void SubmitVoteAsync()
    {
        Guid userId = await GetUserId();

        var voteView = new VoteView()
        {
            Value = this.selectedVoteValue,
            MotionId = this.ActiveMotion.MotionId,
            UserId = userId
        };

        await this.VoteViewService.AddAsync(voteView);
        await this.hubConnection.InvokeAsync(VoteHub.VoteUpdatedMethod);
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