using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Services.Views.Motions;
using Syncfusion.Blazor.Diagrams;

namespace Parlivote.Web.Views.Components.Motions;

public partial class MotionListItem : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Parameter]
    public MotionView Motion { get; set; }

    [Parameter]
    public EventCallback OnMotionChanged { get; set; }

    private string statusPillCss = "";
    private HubConnection hubConnection;
    private EditMotionComponent editMotionComponent;
    private bool existsActiveMeeting = false;
    private bool IsConnected => this.hubConnection.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToMotionHub();
        await GetActiveMotion();
    }

    private async Task GetActiveMotion()
    {
        MotionView activeMotion =
            await this.MotionViewService.GetActiveAsync();

        this.existsActiveMeeting = activeMotion is not null;
    }

    private async Task ConnectToMotionHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.hubConnection.On<MotionView>(MotionHub.SetActiveMotionMethod, motionView =>
        {
            this.existsActiveMeeting = true;
            InvokeAsync(StateHasChanged);
        });

        await this.hubConnection.StartAsync();
    }

    protected override void OnParametersSet()
    {
        this.statusPillCss = GetPillCssByStatus();
    }

    private string GetPillCssByStatus()
    {
        return MotionStateConverter.FromString(Motion.State) switch
        {
            MotionState.Submitted => "bg-primary",
            MotionState.Pending => "bg-warning",
            MotionState.Accepted => "bg-success",
            MotionState.Declined => "bg-danger",
            _ => ""
        };
    }

    private async void SetActive()
    {
        if (IsConnected && !this.existsActiveMeeting)
        {
            await this.hubConnection.SendAsync(MotionHub.SetActiveMotionMethod, Motion);
            Motion.State = MotionState.Pending.GetValue();
            this.statusPillCss = GetPillCssByStatus();
            await InvokeAsync(StateHasChanged);
        }
    }
}