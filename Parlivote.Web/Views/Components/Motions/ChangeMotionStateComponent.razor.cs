using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Motions;

public partial class ChangeMotionStateComponent : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Parameter]
    public MotionView Motion { get; set; }

    private ButtonBase submittedButton;
    private ButtonBase pendingButton;
    private ButtonBase cancelledButton;

    private bool submittedButtonEnabled;
    private bool pendingButtonEnabled;
    private bool cancelledButtonEnabled;
    private bool existsActiveMotion;
    private MotionState initialState = MotionState.Unset;

    protected override async Task OnParametersSetAsync()
    {
        this.initialState = MotionStateConverter.FromString(Motion.State);
        await LoadActiveMotion();
        await SetButtonEnabled();
    }
    private async Task LoadActiveMotion()
    {
        try
        {
            MotionView activeMotion =
                await this.MotionViewService.GetActiveAsync();

            this.existsActiveMotion = activeMotion is not null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SetButtonEnabled()
    {
        MotionState state = MotionStateConverter.FromString(this.Motion.State);
        switch (state)
        {
            case MotionState.Submitted: 
                this.submittedButtonEnabled = false;
                this.pendingButtonEnabled = !this.existsActiveMotion;
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

        await InvokeAsync(StateHasChanged);
    }

    private void SetState(MotionState state)
    {
        this.Motion.State = state.GetValue();
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
}