using System;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Web.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(UserLogin login);
        Task<AuthenticationResult> RegisterAsync(UserRegistration registration);
        Task LogoutAsync(Guid userId);
    }
}