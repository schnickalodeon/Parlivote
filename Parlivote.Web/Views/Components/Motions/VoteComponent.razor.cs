using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes.Exceptions;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Services.Views.Votes;
using Parlivote.Web.Views.Components.Users;
using Syncfusion.Blazor.PivotView.Internal;

namespace Parlivote.Web.Views.Components.Motions;

public partial class VoteComponent: ComponentBase
{
    [Inject]
    public IVoteViewService VoteViewService { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }
    
    [Parameter]
    public MotionView ActiveMotion { get; set; }

    [Parameter]
    public EventCallback<VoteView> AfterVoteSubmitted { get; set; }

    [Parameter]
    public EventCallback<VoteValue> OnSelectedVoteValueChanged { get; set; }

    [Parameter]
    public EventCallback<MotionView> OnVotingFinished { get; set; }

    private Guid userId;

    private HubConnection voteHubHubConnection;
    private HubConnection motionHubConnection;

    private bool mayUserVote;
    private bool hasUserVoted;
    private UserComponent userComponent;
    private bool isVotingFinished;
    private VoteValue selectedVoteValue;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToVoteHub();
        await ConnectToMotionHub();
    }

    protected override async Task OnParametersSetAsync()
    {
        this.userId = await this.userComponent.GetUserId();

        Func<VoteView, bool> userHasVotedCondition =
            vote => vote.UserId == this.userId && vote.Value != VoteValue.NoValue;

        this.hasUserVoted =
            this.ActiveMotion.VoteViews.Any(userHasVotedCondition);

        Func<VoteView, bool> userMayVoteCondition =
            vote => vote.UserId == this.userId;

        this.mayUserVote =
            this.ActiveMotion.VoteViews.Any(userMayVoteCondition);
    }

    private async Task ConnectToMotionHub()
    {
        this.motionHubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        await this.motionHubConnection.StartAsync();
    }
    private async Task ConnectToVoteHub()
    {
        this.voteHubHubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/voteHub"))
            .Build();

        this.voteHubHubConnection.On<VoteView>(VoteHub.VoteUpdatedMethod, async (newVote) =>
        {
            VoteView voteViewToUpdate =
                this.ActiveMotion.VoteViews.Find(vote => vote.UserId == newVote.UserId);

            if (voteViewToUpdate is null)
            {
                throw new NotFoundVoteException();
            }

            voteViewToUpdate.Value = newVote.Value;

            await this.AfterVoteSubmitted.InvokeAsync(newVote);
            await InvokeAsync(StateHasChanged);
        });

        this.voteHubHubConnection.On<MotionView>(VoteHub.VoteFinishedMethod, async (finishedMotionView) =>
        {
            await this.OnVotingFinished.InvokeAsync(finishedMotionView);
        });

        await this.voteHubHubConnection.StartAsync();
    }

    private async void SetVoteValue(VoteValue value)
    {
        this.selectedVoteValue = value;
        await this.OnSelectedVoteValueChanged.InvokeAsync(value);
        StateHasChanged();
    }
  
    public async Task SubmitVoteAsync()
    {
        VoteView submittedVote =
            this.ActiveMotion.VoteViews.Find(vote => vote.UserId == this.userId);

        if (submittedVote is null)
        {
            throw new NotFoundVoteException();
        }

        submittedVote.Value = this.selectedVoteValue;

        VoteView updatedVote =
            await this.VoteViewService.UpdateAsync(submittedVote);

        this.hasUserVoted = true;
        await this.voteHubHubConnection.InvokeAsync(VoteHub.VoteUpdatedMethod, updatedVote);

        if (IsVotingFinished())
        {
            MotionView finishedMotion = await GetUpdatedMotionResult();
            await this.voteHubHubConnection.InvokeAsync(VoteHub.VoteFinishedMethod, finishedMotion);
            await this.motionHubConnection.InvokeAsync(MotionHub.SetStateMethod, finishedMotion);
        }
    }

    private async Task<MotionView> GetUpdatedMotionResult()
    {
        MotionState state = GetMotionState();
        this.ActiveMotion.State = state.GetValue();

        MotionView updatedMotion =
            await this.MotionViewService.UpdateAsync(this.ActiveMotion);

        return updatedMotion;
    }

    private MotionState GetMotionState()
    {
        List<VoteView> votes = this.ActiveMotion.VoteViews;
        float yesCount = votes.Count(vote => vote.Value == VoteValue.For);

        //TODO Später auslagern
        bool IsAccepted(int all, float yes) => (yes / all) > 0.5;

        bool isMotionAccepted = IsAccepted(votes.Count, yesCount);

        return isMotionAccepted ? MotionState.Accepted : MotionState.Declined;
    }

    private bool IsVotingFinished()
    {
        List<VoteView> votes = this.ActiveMotion.VoteViews;
        int voteCount = votes.Count(vote => vote.Value != VoteValue.NoValue);
        int attendantCount = votes.Count;

        return voteCount == attendantCount;
    }
}