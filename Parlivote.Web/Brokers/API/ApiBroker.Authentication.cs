using System.Net.Http;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string IdentityRelativeUrl = "/api/v1/identity";
    public async Task<AuthenticationResult> PostLoginAsync(UserLogin userLogin) =>
        await this.PostAsync<UserLogin,AuthenticationResult>($"{IdentityRelativeUrl}/login", userLogin);

    public async Task<AuthenticationResult> PostRegisterAsync(UserRegistration registration) =>
        await this.PostAsync<UserRegistration, AuthenticationResult>($"{IdentityRelativeUrl}/register", registration);
}