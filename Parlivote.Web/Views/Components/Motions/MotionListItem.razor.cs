using System;
using System.Threading.Tasks;
using FluentAssertions.Equivalency;
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
    private MotionResultDialog motionResultDialog;
    private MotionDetailComponent motionDetailComponent;
    private bool existsActiveMeeting = false;
    private bool resultsAvailable = false;
    private bool IsConnected => this.hubConnection.State == HubConnectionState.Connected;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ConnectToMotionHub();
            await GetActiveMotion();
        }

        this.resultsAvailable =
            (Motion is not null &&
             (Motion.State is MotionStateConverter.Accepted or MotionStateConverter.Declined));
    }
    protected override void OnParametersSet()
    {
        this.statusPillCss = GetPillCssByStatus();
    }
    private async Task ConnectToMotionHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.hubConnection.On<MotionView>(MotionHub.SetStateMethod, motionView =>
        {
            if (this.Motion.MotionId == motionView.MotionId)
            {
                //this.existsActiveMeeting = (motionView.State == MotionStateConverter.Pending);
                this.Motion.State = motionView.State;
                this.statusPillCss = GetPillCssByStatus();
                InvokeAsync(StateHasChanged);
            }
        });

        await this.hubConnection.StartAsync();
    }
    private async Task GetActiveMotion()
    {
        MotionView activeMotion =
            await this.MotionViewService.GetActiveAsync();

        this.existsActiveMeeting = activeMotion is not null;
        await InvokeAsync(StateHasChanged);
    }

    private void ShowResult()
    {
        if (!this.resultsAvailable)
        {
            return;
        }

        this.motionResultDialog.Show();
    }

    private string GetPillCssByStatus()
    {
        return MotionStateConverter.FromString(Motion.State) switch
        {
            MotionState.Submitted => "bg-primary",
            MotionState.Pending => "bg-warning",
            MotionState.Accepted => "bg-success",
            MotionState.Cancelled => "bg-secondary",
            MotionState.Declined => "bg-danger",
            _ => ""
        };
    }
}