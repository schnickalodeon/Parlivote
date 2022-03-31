using System.Collections;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class AddMeetingComponent
{
    [Inject]
    public IMeetingViewService MeetingViewServiceService { get; set; }

    [Parameter]
    public EventCallback OnMeetingAdded { get; set; }

    private IDictionary validationErrors;
    private MeetingView meetingView = new();
    private DialogBase dialog;
    private string error;

    public void Show()
    {
        this.meetingView = new MeetingView();
        this.dialog.Show();
    }

    private async void AddMeetingViewAsync()
    {
        try
        {
            await MeetingViewServiceService.AddAsync(this.meetingView);
            await OnMeetingAdded.InvokeAsync();
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