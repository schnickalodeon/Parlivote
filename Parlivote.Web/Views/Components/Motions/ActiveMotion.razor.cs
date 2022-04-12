using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
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

    private MotionView activeMotion;
    private HubConnection hubConnection;
    public bool IsConnected => 
        this.hubConnection.State == HubConnectionState.Connected; 

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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

            this.activeMotion = (motion.State == MotionStateConverter.Pending)
                ? motion
                : null;

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
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}