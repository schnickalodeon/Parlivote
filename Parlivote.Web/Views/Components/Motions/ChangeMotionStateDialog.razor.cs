using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Motions;

public partial class ChangeMotionStateDialog : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private MotionView motion;
    private MotionState stateToUpdate;
    private DialogBase dialog;

    private ButtonBase submittedButton;
    private ButtonBase pendingButton;
    private ButtonBase cancelledButton;

    private bool submittedButtonEnabled;
    private bool pendingButtonEnabled;
    private bool cancelledButtonEnabled;

    private HubConnection hubConnection;
    private bool IsConnected => this.hubConnection.State == HubConnectionState.Connected;

    public void Show(MotionView motion)
    {
        this.motion = motion;
        this.stateToUpdate = MotionStateConverter.FromString(this.motion.State);
        SetButtonEnabled();
        this.dialog.Show();
    }

    protected override async Task OnInitializedAsync()
    {
        await ConnectToMotionHub();
    }

    private void SetButtonEnabled()
    {
        switch (this.stateToUpdate)
        {
            case MotionState.Submitted: 
                this.submittedButtonEnabled = false;
                this.pendingButtonEnabled = true;
                this.cancelledButtonEnabled = true;
                break;

            case MotionState.Pending:
                this.pendingButtonEnabled = false;
                this.submittedButtonEnabled = true;
                this.cancelledButtonEnabled = true;
                break;
            case MotionState.Accepted:
                this.submittedButtonEnabled = false;
                this.pendingButtonEnabled = false;
                this.cancelledButtonEnabled = false;
                break;
            case MotionState.Declined:
                this.submittedButtonEnabled = false;
                this.pendingButtonEnabled = false;
                this.cancelledButtonEnabled = false;
                break;
            case MotionState.Cancelled:
                this.submittedButtonEnabled = true;
                this.pendingButtonEnabled = false;
                this.cancelledButtonEnabled = false;
                break;
            default:
                throw new InvalidEnumArgumentException("This is an invalid argument for the motion state");
        }
    }

    private async Task ConnectToMotionHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        await this.hubConnection.StartAsync();
    }

    private void SetState(MotionState state)
    {
        this.stateToUpdate = state;
        StateHasChanged();
    }

    private string GetPillCssByStatus(string state)
    {
        return GetPillCssByStatus(MotionStateConverter.FromString(state));
    }

    private string GetPillCssByStatus(MotionState state)
    {
        return state switch
        {
            MotionState.Submitted => "bg-primary",
            MotionState.Pending => "bg-warning",
            MotionState.Accepted => "bg-success",
            MotionState.Cancelled => "bg-secondary",
            MotionState.Declined => "bg-danger",
            _ => ""
        };
    }

    private async Task SaveState()
    {
        if (IsConnected)
        {
            motion.State = this.stateToUpdate.GetValue();
            await this.hubConnection.SendAsync(MotionHub.SetStateMethod, motion);
            this.dialog.Hide();
        }
    }

}