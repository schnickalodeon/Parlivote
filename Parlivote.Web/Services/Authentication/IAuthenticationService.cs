using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Web.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(UserLogin userForAuth);
        Task LogoutAsync();
        //Task<AuthenticationResult> Register(UserRegistration user);
    }
}