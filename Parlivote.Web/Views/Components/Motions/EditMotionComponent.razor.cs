using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Motions.Exceptions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Base;
using System;
using System.Collections;

namespace Parlivote.Web.Views.Components.Motions;

public partial class EditMotionComponent
{
    [Inject]
    public IMotionViewService MotionViewServiceService { get; set; }

    [Parameter]
    public EventCallback OnMotionSaved { get; set; }

    private IDictionary validationErrors;
    private MotionView motionView = new();
    private DialogBase dialog;
    private string error;

    public void Show(MotionView motionToEdit)
    {
        this.motionView = motionToEdit;
        this.dialog.Show();
    }

    private async void SaveMotionViewAsync()
    {
        try
        {
            await MotionViewServiceService.UpdateAsync(this.motionView);
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