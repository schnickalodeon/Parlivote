using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Services.Identity;

public interface IIdentityService
{
    Task<AuthenticationResult> RegisterAsync(string userRegistrationEmail, string userRegistrationPassword);
}