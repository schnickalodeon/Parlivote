using System.Collections;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class EditMeetingComponent
{
    [Inject]
    public IMeetingViewService MeetingViewServiceService { get; set; }

    [Parameter]
    public EventCallback OnMeetingChanged { get; set; }

    private IDictionary validationErrors;
    private MeetingView meetingToEdit = new();
    private DialogBase dialog;
    private string error;

    public void Show(MeetingView meeting)
    {
        this.meetingToEdit = meeting;
        this.dialog.Show();
    }

    private async void SaveMeetingViewAsync()
    {
        try
        {
            await MeetingViewServiceService.UpdateAsync(this.meetingToEdit);
            await OnMeetingChanged.InvokeAsync();
            ClearAndHide();
        }
        catch (MeetingValidationException meetingValidationException)
        {
            var invalidMeetingException =
                meetingValidationException.InnerException;

            this.validationErrors =
                invalidMeetingException?.Data;

            string validationMessage =
                invalidMeetingException?.Message;

            this.error = validationMessage;
        }
    }

    private void ClearAndHide()
    {
        this.dialog.Hide();
    }
}