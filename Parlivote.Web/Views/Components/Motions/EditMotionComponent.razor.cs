using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Motions.Exceptions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Base;
using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Web.Hubs;

namespace Parlivote.Web.Views.Components.Motions;

public partial class EditMotionComponent
{
    [Inject]
    public IMotionViewService MotionViewServiceService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public EventCallback OnMotionSaved { get; set; }

    private IDictionary validationErrors;
    private MotionView motionView = new();
    private HubConnection hubConnection;
    private DialogBase dialog;
    private string error;

    protected override async Task OnInitializedAsync()
    {
        await ConnectToMotionHub();
    }

    public void Show(MotionView motionToEdit)
    {
        this.motionView = motionToEdit;
        this.dialog.Show();
    }

    private async Task ConnectToMotionHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/motionhub"))
            .Build();

        await this.hubConnection.StartAsync();
    }

    private async void SaveMotionViewAsync()
    {
        try
        {
            this.motionView = 
                await MotionViewServiceService.UpdateAsync(this.motionView);

            await this.hubConnection.SendAsync(MotionHub.SetStateMethod, this.motionView);
            await OnMotionSaved.InvokeAsync();
            ClearAndHide();
        }
        catch (MotionValidationException motionValidationException)
        {
            Exception invalidMotionException =
                motionValidationException.InnerException;

            this.validationErrors =
                invalidMotionException?.Data;

            string validationMessage =
                invalidMotionException?.Message;

            this.error = validationMessage;
        }
    }

    private void ClearAndHide()
    {
        this.dialog.Hide();
    }
}