using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Polls;
using Parlivote.Web.Services.Views.Polls;

namespace Parlivote.Web.Components.Polls;

public partial class PollList : ComponentBase
{
    [Inject]
    public IPollViewService PollViewService { get; set; }

    private ComponentState state;
    private List<PollView> polls;
    private string error;

    protected override async Task OnInitializedAsync()
    {
        this.state = ComponentState.Loading;
        await LoadPolls();
    }

    private async Task LoadPolls()
    {
        try
        {
            this.polls = await this.PollViewService.GetAllAsync();
            this.state = ComponentState.Content;
        }
        catch (Exception e)
        {
            this.error = e.InnerException.Message;
            this.state = ComponentState.Error;
        }
        StateHasChanged();
    }
}