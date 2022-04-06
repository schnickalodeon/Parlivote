using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Views.Components.Motions;

public partial class ActiveMotion : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    //TODO Get ActiveNotion from DB

    private MotionView activeMotion;
    private HubConnection hubConnetion;
    public bool IsConnected => hubConnetion.State == HubConnectionState.Connected; 

    protected override async Task OnInitializedAsync()
    {
        this.hubConnetion = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.hubConnetion.On<MotionView>(MotionHub.SetActiveMotionMethod, (motion) =>
        {
            this.activeMotion = motion;
            InvokeAsync(StateHasChanged);
        });

        await this.hubConnetion.StartAsync();
    }

    private async Task LoadActiveMotionAsnyc()
    {
        try
        {
            this.activeMotion =
                await MotionViewService.GetActiveAsync();
        }
        catch (Exception e)
        {
            
        }
    }
}