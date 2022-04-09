using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingDropDown : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Parameter]
    public Guid? MeetingId { get; set; }

    private Guid? BoundValue
    {
        get => this.MeetingId;
        set => MeetingIdChanged.InvokeAsync(value);
    }

    [Parameter]
    public EventCallback<Guid?> MeetingIdChanged { get; set; }

    [Parameter]
    public EventCallback InContentState { get; set; }

    private ComponentState state;
    private string error;
    private List<MeetingView> meetings;

    protected override async Task OnInitializedAsync()
    {
        await LoadMeetings();
    }

    private async Task LoadMeetings()
    {
        try
        {
            SetState(ComponentState.Loading);

            this.meetings =
                await this.MeetingViewService.GetAllAsync();

            AddNullMeeting();

            SetState(ComponentState.Content);
        }
        catch (Exception e)
        {
            this.error = e.Message;
            this.state = ComponentState.Error;
        }
    }

    private void AddNullMeeting()
    {
        var nullMeetingView = new MeetingView
        {
            Description = "---",
            Id = null
        };
        this.meetings.Insert(0, nullMeetingView);
    }

    private void SetState(ComponentState newState)
    {
        if (newState == ComponentState.Content)
        {
            this.InContentState.InvokeAsync();
        }

        this.state = newState;
    }

}