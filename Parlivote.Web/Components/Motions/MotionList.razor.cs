using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Components.Motions;

public partial class MotionList : ComponentBase
{
    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    private ComponentState state;
    private List<MotionView> motions;
    private string error;

    protected override async Task OnInitializedAsync()
    {
        this.state = ComponentState.Loading;
        await LoadMotions();
    }

    private async Task LoadMotions()
    {
        try
        {
            this.motions = await this.MotionViewService.GetAllAsync();
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