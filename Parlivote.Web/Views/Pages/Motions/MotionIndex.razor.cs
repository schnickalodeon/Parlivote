using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Views.Pages.Motions;

public partial class MotionIndex : ComponentBase
{
    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    private ComponentState state;

    [Parameter]
    public List<MotionView> Motions { get; set; } = null;

    [Parameter]
    public EventCallback OnMotionsChanged { get; set; }

    private string error;

    protected override async Task OnParametersSetAsync()
    {
        if (Motions is null)
        {
            this.state = ComponentState.Loading;
            await LoadMotions();
        }
        else
        {
            this.state = ComponentState.Content;
        }
    }

    private async Task LoadMotions()
    {
        try
        {
            this.Motions = await this.MotionViewService.GetAllWithMeetingAsync();
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