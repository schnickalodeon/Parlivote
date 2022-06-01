using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Parlivote.Web.Services.Authentication;

namespace Parlivote.Web.Views.Shared;

public partial class LoginDisplay
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public IAuthenticationService AuthenticationService { get; set; }
    private async Task LogOut()
    {
        Guid userId = await GetUserId();
        await this.AuthenticationService.LogoutAsync(userId);
    }

    private async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string userId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(userId);
    }
}