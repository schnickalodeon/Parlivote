using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Motions.Exceptions;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Motions;

public partial class DeleteMotionDialog
{
    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    private ConfirmationDialog confirmationDialog;
    private MotionView motionViewToDelete;
    private string error;
    private async Task DeleteMotion()
    {
        try
        {
            Guid motionIdToDelete = this.motionViewToDelete.MotionId;
            await this.MotionViewService.RemoveByIdAsync(motionIdToDelete);
            await CloseAndReportSuccess();
        }
        catch (MotionValidationException motionViewValidationException)
        {
            string validationMessage =
                motionViewValidationException?.InnerException?.Message;

            this.error = validationMessage;
        }
        catch (MotionDependencyValidationException motionViewDependencyValidationException)
        {
            string validationMessage =
                motionViewDependencyValidationException?.InnerException?.Message;

            this.error = validationMessage;
        }
        catch (MotionDependencyException motionViewDependencyException)
        {
            string validationMessage =
                motionViewDependencyException.Message;

            this.error = validationMessage;
        }
        catch (MotionServiceException motionViewServiceException)
        {
            string validationMessage =
                motionViewServiceException.Message;

            this.error = validationMessage;
        }
    }

    private async Task CloseAndReportSuccess()
    {
        this.confirmationDialog.Hide();
        await this.OnDeleted.InvokeAsync();
    }

    public void Show(MotionView motionView)
    {
        this.motionViewToDelete = motionView;
        this.confirmationDialog.Show();
    }
}