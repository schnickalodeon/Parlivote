using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingList : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    private ComponentState state;
    private List<MeetingView> meetings;
    private string error;

    protected override async Task OnInitializedAsync()
    {
        this.state = ComponentState.Loading;
        await LoadMeetings();
    }

    private async Task LoadMeetings()
    {
        try
        {
            this.meetings =
                await this.MeetingViewService.GetAllAsync();
            this.state = ComponentState.Content;
        }
        catch (Exception e)
        {
            this.error = e.InnerException?.Message;
            this.state = ComponentState.Error;
        }
    }
}