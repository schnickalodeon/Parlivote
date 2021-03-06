using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;

namespace Parlivote.Web.Views.Components.Meetings;

public partial class MeetingList : ComponentBase
{
    [Inject]
    public HttpClient Client { get; set; }

    [Inject]
    public IMeetingViewService MeetingViewService { get; set; }

    private AddMeetingComponent addMeetingComponent;
    private HubConnection hubConnection;
    private ComponentState state;
    private List<MeetingView> meetings;
    private string error;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.state = ComponentState.Loading;
            await LoadMeetings();
        }
    }

    public async Task LoadMeetings()
    {
        try
        {
            this.meetings =
                await this.MeetingViewService.GetAllWithMotionsAsync();

            this.state = ComponentState.Content;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            this.error = e.InnerException?.Message;
            this.state = ComponentState.Error;
        }
    }
}