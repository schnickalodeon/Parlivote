using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Syncfusion.Blazor.Diagrams;

namespace Parlivote.Web.Views.Components.Motions;

public partial class MotionListItem : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public MotionView Motion { get; set; }

    [Parameter]
    public EventCallback OnMotionChanged { get; set; }

    private string statusPillCss = "";
    private HubConnection hubConnetion;
    private EditMotionComponent editMotionComponent;
    private bool IsConnected => this.hubConnetion.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        this.hubConnetion = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        this.hubConnetion.On<string>(MotionHub.SetActiveMotionMethod, (motion) =>
        {
            Console.WriteLine(motion);
            StateHasChanged();
        });

        await this.hubConnetion.StartAsync();
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
        if (IsConnected)
        {
            await this.hubConnetion.SendAsync(MotionHub.SetActiveMotionMethod, Motion);
        }
    }
}