using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Parlivote.Web.Views.Components.Users;

public partial class UserComponent : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    public async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string strUserId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(strUserId);
    }
}