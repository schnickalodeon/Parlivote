using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Motions.Exceptions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Base;
using System;
using System.Collections;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Views.Components.Users;

namespace Parlivote.Web.Views.Components.Motions;

public partial class AddMotionComponent
{
    [Inject]
    public IMotionViewService MotionViewServiceService { get; set; }

    [Parameter]
    public EventCallback OnMotionAdded { get; set; }

    private IDictionary validationErrors;
    private MotionView motionView = new();
    private UserComponent userComponent;
    private DialogBase dialog;
    private string error;
    private string role;
    
    public async Task Show(MeetingView meeting = null, int version = 1)
    {
        Guid? applicantId = await GetApplicantId();

        this.motionView = new MotionView()
        {
            ApplicantId = applicantId,
            MeetingId = meeting?.Id,
        };

        this.dialog.Show();
    }

    private async Task<Guid?> GetApplicantId()
    {
        this.role = await this.userComponent.GetUserRole();

        if (this.role == Roles.APPLICANT)
        {
            return await this.userComponent.GetUserId();
        }

        return null;
    }

    private async void AddMotionViewAsync()
    {
        try
        {
            await MotionViewServiceService.AddAsync(this.motionView);
            await OnMotionAdded.InvokeAsync();
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