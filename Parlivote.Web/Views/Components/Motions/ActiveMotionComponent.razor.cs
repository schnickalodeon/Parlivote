using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Services.Views.Votes;
using Parlivote.Web.Views.Components.Users;

namespace Parlivote.Web.Views.Components.Motions;

public partial class ActiveMotionComponent : ComponentBase
{

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private VoteComponent voteComponent;
    private MotionView activeMotion;
    private bool isVotingFinished = false;
    private int voteCount = 0;
    private string submitButtonCss;
    private int attendanceCount = 0;

    private HubConnection motionHubConnection;
    
    private MotionView finishedMotion;
    private MotionResultDialog motionResultDialog;
    private UserComponent userComponent;
    private Guid userId;

    protected override async Task OnInitializedAsync()
    {
        await SetSubmitButtonCss(VoteValue.NoValue);
        await ConnectToMotionHub();
        await LoadActiveMotionAsync();
        this.userId = await this.userComponent.GetUserId();
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
                this.isVotingFinished = false;
                this.activeMotion = motion;
                this.voteCount = motion.VoteViews.Count(vote => vote.Value != VoteValue.NoValue);
                this.attendanceCount = motion.VoteViews.Count;
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
                this.voteCount = this.activeMotion.VoteViews.Count(vote => vote.Value != VoteValue.NoValue);
                this.attendanceCount = this.activeMotion.VoteViews.Count;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async Task UpdateVote(VoteView vote)
    {
        this.voteCount = 
            this.activeMotion.VoteViews.Count(vote => vote.Value != VoteValue.NoValue);

        await InvokeAsync(StateHasChanged);

        if (vote.UserId == this.userId)
        {
            await SetSubmitButtonCss(VoteValue.NoValue);
        }

    }

    private async Task OnVotingFinished(MotionView finishedMotionView)
    {
        this.finishedMotion = finishedMotionView;
        this.activeMotion = null;
        this.isVotingFinished = true;
        await InvokeAsync(StateHasChanged);
    }

    private MotionState GetMotionResult()
    {
        List<VoteView> votes = this.finishedMotion.VoteViews;
        float yesCount = votes.Count(vote => vote.Value == VoteValue.For);

        //TODO Später auslagern
        bool IsAccepted(int all, float yes) => (yes / all) > 0.5;

        bool isMotionAccepted = IsAccepted(votes.Count, yesCount);

        return isMotionAccepted ? MotionState.Accepted : MotionState.Declined;
    }

    private async Task SetSubmitButtonCss(VoteValue selectedVoteValue)
    {
        this.submitButtonCss = selectedVoteValue switch
        {
            VoteValue.For => "btn btn-success",
            VoteValue.Against => "btn btn-danger",
            VoteValue.Abstention => "btn btn-warning",
            _ => "invisible"
        };
        await InvokeAsync(StateHasChanged);
    }
  
}