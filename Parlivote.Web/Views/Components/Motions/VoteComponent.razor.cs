using System;
using System.Collections.Generic;
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
using Syncfusion.Blazor.PivotView.Internal;

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
    private bool isVotingFinished = false;
    private Guid userId;
    private int voteCount = 0;
    private int attendanceCount = 0;
    private VoteValue selectedVoteValue = VoteValue.NoValue;
    private HubConnection motionHubConnection;
    private HubConnection voteHubHubConnection;
    private MotionView finishedMotion;
    private MotionResultDialog motionResultDialog;

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

        this.voteHubHubConnection.On<VoteView, bool>(VoteHub.VoteUpdatedMethod, async (voteView, votingIsFinished) =>
        {
            if (votingIsFinished)
            {
                if (voteView.UserId != this.userId)
                {
                    this.activeMotion.VoteViews.Add(voteView);
                }
                this.finishedMotion = this.activeMotion;
                this.activeMotion = null;
                this.isVotingFinished = true;
            }
            else
            {
                await LoadActiveMotionAsync();
            }

            await InvokeAsync(StateHasChanged);
        });

        await this.voteHubHubConnection.StartAsync();
    }
    private async Task ConnectToMotionHub()
    {
        this.motionHubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.motionHubConnection.On<MotionView>(MotionHub.SetStateMethod, async (motion) =>
        {

            if (motion.State == MotionStateConverter.Pending)
            {
                this.activeMotion = motion;
                this.hasUserVoted = motion.VoteViews.Any(vote => vote.UserId == this.userId);
            }
            
            await InvokeAsync(StateHasChanged);
        });

        await this.motionHubConnection.StartAsync();
    }
    private async Task LoadActiveMotionAsync()
    {
        try
        {
            this.activeMotion =
                await MotionViewService.GetActiveAsync();

            if (this.activeMotion is not null)
            {

                this.attendanceCount = this.activeMotion.MeetingAttendanceCount;
                this.voteCount = this.activeMotion.VoteViews.Count;

                this.hasUserVoted =
                    this.activeMotion.VoteViews.Any(motion => motion.UserId == this.userId);

                await InvokeAsync(StateHasChanged);
            }
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

        this.selectedVoteValue = VoteValue.NoValue;
        this.hasUserVoted = true;

        int isCount = this.activeMotion.VoteViews.Count + 1;
        int mustCount = this.activeMotion.MeetingAttendanceCount;

        bool votingIsFinished = isCount == mustCount;
        if (votingIsFinished)
        {
            await SetMotionResult();
        }

        await this.voteHubHubConnection.InvokeAsync(VoteHub.VoteUpdatedMethod, submittedVote, votingIsFinished);
    }

    private async Task SetMotionResult()
    {
        await LoadActiveMotionAsync();
        MotionState motionResult = GetMotionResult();
        this.activeMotion.State = motionResult.GetValue();

        await this.MotionViewService.UpdateAsync(this.activeMotion);
        await this.motionHubConnection.InvokeAsync(MotionHub.SetStateMethod, this.activeMotion);
    }

    private MotionState GetMotionResult()
    {
        List<VoteView> votes = this.activeMotion.VoteViews;
        float yesCount = votes.Count(vote => vote.Value == VoteValue.For);

        //TODO Später auslagern
        bool IsAccepted(int all, float yes) => (yes / all) > 0.5;

        bool isMotionAccepted = IsAccepted(votes.Count, yesCount);

        return isMotionAccepted ? MotionState.Accepted : MotionState.Declined;
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