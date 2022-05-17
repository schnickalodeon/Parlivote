using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Services.Views.Votes;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Motions;

public partial class ActiveMotion : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Inject]
    public IVoteViewService VoteViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private MotionView activeMotion;
    private HubConnection hubConnection;
    private Guid userId;
    private bool hasUserVoted = false;
    public bool IsConnected => 
        this.hubConnection.State == HubConnectionState.Connected; 

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.userId = await GetUserId();
            await ConnectToMotionHub();
            await LoadActiveMotionAsync();
        }
    }

    private async Task ConnectToMotionHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.hubConnection.On<MotionView>(MotionHub.SetStateMethod, (motion) =>
        {

            if (motion.State == MotionStateConverter.Pending)
            {
                this.activeMotion = motion;
                this.hasUserVoted = motion.VoteViews.Any(vote => vote.UserId == userId);
            }
            else
            {
                this.activeMotion = null;
            }

            InvokeAsync(StateHasChanged);
        });

        await this.hubConnection.StartAsync();
    }

    private async Task LoadActiveMotionAsync()
    {
        try
        {
            this.activeMotion =
                await MotionViewService.GetActiveAsync();

            this.hasUserVoted = 
                this.activeMotion.VoteViews.Any(vote => vote.UserId == this.userId);

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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

    private void SetVoted()
    {
        this.hasUserVoted = true;
        StateHasChanged();
    }
}