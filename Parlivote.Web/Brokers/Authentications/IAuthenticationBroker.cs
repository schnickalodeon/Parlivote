using System.Net.Http.Headers;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Web.Brokers.Authentications;

public interface IAuthenticationBroker
{
    Task<AuthenticationResult> PostLoginAsync(UserLogin userLogin);
    Task<AuthenticationResult> PostRegisterAsync(UserRegistration registration);
    Task<AuthenticationResult> PostRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task<AuthenticationHeaderValue> GetAuthorizationHeaderAsync();
}