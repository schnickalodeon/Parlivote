﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Views.Base;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingBox : ComponentBase
{
    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    [Parameter] 
    public EventCallback OnDeleted { get; set; }

    [Parameter]
    public MeetingView Meeting { get; set; }

    private ConfirmationDialog confirmationDialog;

    private async void DeleteMeeting()
    {
        try
        {
            await this.MeetingViewService.DeleteByIdAsync(Meeting.Id);
            await OnDeleted.InvokeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}