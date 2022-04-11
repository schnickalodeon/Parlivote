using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Services.Identity;

public interface IIdentityService
{
    Task<AuthSuccessResponse> RegisterAsync(string email, string password);
    Task<AuthSuccessResponse> LoginAsync(string email, string password);
    Task<AuthSuccessResponse> RefreshTokenAsync(string token, string refreshToken);
}