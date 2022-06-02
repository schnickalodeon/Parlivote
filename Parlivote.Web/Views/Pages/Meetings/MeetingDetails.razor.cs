using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Server.IIS.Core;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Pages.Meetings;

public partial class MeetingDetails : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    private MeetingView meeting;
    private string error;
    private ComponentState state;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.state = ComponentState.Loading;
            await LoadMeetingDataAsync();
        }
    }

    private async Task LoadMeetingDataAsync()
    {
        try
        {
            this.meeting =
                await this.MeetingViewService.GetByIdWithMotions(Id);
            this.state = ComponentState.Content;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            this.error = e.Message;
            this.state = ComponentState.Error;
        }
    }
}