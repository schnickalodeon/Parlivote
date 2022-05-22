using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Services.Views.Votes;

namespace Parlivote.Web.Views.Components.Motions;

public partial class VoteComponent: ComponentBase
{
    [Inject]
    public IVoteViewService VoteViewService { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private MotionView activeMotion;
    private bool hasUserVoted;
    private Guid userId;
    private int attendanceCount = 0;
    private VoteValue selectedVoteValue = VoteValue.NoValue;
    private HubConnection motionHubConnection;
    private HubConnection voteHubHubConnection;

    protected override async Task OnInitializedAsync()
    {
        this.userId = await GetUserId();
        await ConnectToVoteHub();
        await ConnectToMotionHub();
        await LoadActiveMotionAsync();
    }
    private async Task ConnectToVoteHub()
    {
        this.voteHubHubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/voteHub"))
            .Build();

        this.voteHubHubConnection.On<MeetingView>(VoteHub.AttendanceUpdatedMethod, (meetingView) =>
        {
            if (this.activeMotion.MeetingId == meetingView.Id)
            {
                this.attendanceCount = meetingView.Attendances.Count;
                InvokeAsync(StateHasChanged);
            }
        });

        this.voteHubHubConnection.On<VoteView>(VoteHub.VoteUpdatedMethod, async (voteView) =>
        {
            this.hasUserVoted = voteView.UserId == this.userId;

            CheckVotingFinished();

            await InvokeAsync(StateHasChanged);
        });

        await this.voteHubHubConnection.StartAsync();
    }
    private async Task ConnectToMotionHub()
    {
        this.motionHubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.motionHubConnection.On<MotionView>(MotionHub.SetStateMethod, (motion) =>
        {

            if (motion.State == MotionStateConverter.Pending)
            {
                this.activeMotion = motion;
                this.hasUserVoted = motion.VoteViews.Any(vote => vote.UserId == this.userId);
            }
            else
            {
                this.activeMotion = null;
            }

            InvokeAsync(StateHasChanged);
        });

        await this.motionHubConnection.StartAsync();
    }

    private async Task LoadActiveMotionAsync()
    {
        try
        {
            this.activeMotion =
                await MotionViewService.GetActiveAsync();

            this.attendanceCount = this.activeMotion.MeetingAttendanceCount;

            this.hasUserVoted =
                this.activeMotion.VoteViews.Any(motion => motion.UserId == this.userId);

            CheckVotingFinished();

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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
    private async void SubmitVoteAsync()
    {
        var voteView = new VoteView()
        {
            Value = this.selectedVoteValue,
            MotionId = this.activeMotion.MotionId,
            UserId = userId
        };

        VoteView submittedVote =
            await this.VoteViewService.AddAsync(voteView);

        await this.voteHubHubConnection.InvokeAsync(VoteHub.VoteUpdatedMethod, submittedVote);
    }

    private void CheckVotingFinished()
    {
        if (this.activeMotion.VoteViews.Count == this.attendanceCount)
        {
            //TODO Call ResultComponent
        }
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
    private async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string strUserId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(strUserId);
    }
}