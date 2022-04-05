﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Views.Base;
using Parlivote.Web.Views.Components.Motions;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingBox : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter] 
    public EventCallback OnMeetingChanged { get; set; }

    [Parameter]
    public MeetingView Meeting { get; set; }

    private ConfirmationDialog confirmationDialog;
    private AddMotionComponent addMotionComponent;
    private EditMeetingComponent editMeetingComponent;

    private async void DeleteMeeting()
    {
        try
        {
            await this.MeetingViewService.DeleteByIdAsync(Meeting.Id);
            await OnMeetingChanged.InvokeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}