using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Web.Services.Authentication;

namespace Parlivote.Web.Views.Shared;

public partial class LoginDisplay
{
    [Inject]
    public IAuthenticationService AuthenticationService { get; set; }
    private async Task LogOut()
    {
        await this.AuthenticationService.LogoutAsync();
    }
}