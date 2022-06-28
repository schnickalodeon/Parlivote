using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Models;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Views.Components.Motions;
using Parlivote.Web.Views.Components.Users;

namespace Parlivote.Web.Views.Pages.Motions;

public partial class MotionIndex : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public IMotionViewService MotionViewService { get; set; }

    private ComponentState state;
    private AddMotionComponent addMotionComponent;
    private EditMotionComponent editMotionComponent;
    private DeleteMotionDialog deleteMotionComponent;

    private List<MotionView> motions;

    private readonly List<string> fieldsToIgnore = new()
    {
        ("Text")
    };

    private string error;

    protected override async Task OnInitializedAsync()
    {
        await LoadMotions();
    }

    private async Task LoadMotions()
    {
        try
        {
            string role = await GetUserRole();
            Guid userId = await GetUserId();

            this.motions = (role == Roles.APPLICANT)
                ? await this.MotionViewService.GetMyWithMeetingAsync(userId)
                : await this.MotionViewService.GetAllWithMeetingAsync();

            this.state = ComponentState.Content;
        }
        catch (Exception e)
        {
            this.error = e.InnerException.Message;
            this.state = ComponentState.Error;
        }
        StateHasChanged();
    }

    public async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string strUserId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(strUserId);
    }

    public async Task<string> GetUserRole()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string userRole =
            authState.User.Claims.First(claim => claim.Type == "role").Value;

        return userRole;
    }
}