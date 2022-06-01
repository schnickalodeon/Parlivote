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
using Syncfusion.Blazor.PivotView.Internal;

namespace Parlivote.Web.Views.Components.Motions;

public partial class VoteComponent: ComponentBase
{
    [Inject]
    public IVoteViewService VoteViewService { get; set; }

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

    private MotionView finishedMotion;
    private HubConnection voteHubHubConnection;
    private UserComponent userComponent;
    private bool hasUserVoted;
    private bool isVotingFinished;
    private VoteValue selectedVoteValue;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToVoteHub();
    }

    protected override async Task OnParametersSetAsync()
    {
        this.userId = await this.userComponent.GetUserId();

        Func<VoteView, bool> userHasVotedCondition =
            vote => vote.UserId == this.userId && vote.Value != VoteValue.NoValue;

        this.hasUserVoted =
            this.ActiveMotion.VoteViews.Any(userHasVotedCondition);
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

        this.voteHubHubConnection.On(VoteHub.VoteFinishedMethod, async () =>
        {
            //this.finishedMotion = finishedMotion;
            //this.ActiveMotion = null;
            //this.isVotingFinished = true;
            await this.OnVotingFinished.InvokeAsync(this.ActiveMotion);
            //await InvokeAsync(StateHasChanged);
        });

        await this.voteHubHubConnection.StartAsync();
    }

    private async Task SetVoteValue(VoteValue value)
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
            await this.voteHubHubConnection.InvokeAsync(VoteHub.VoteFinishedMethod);
        }
    }

    private bool IsVotingFinished()
    {
        List<VoteView> votes = this.ActiveMotion.VoteViews;
        int voteCount = votes.Count(vote => vote.Value != VoteValue.NoValue);
        int attendantCount = votes.Count;

        return voteCount == attendantCount;
    }
}