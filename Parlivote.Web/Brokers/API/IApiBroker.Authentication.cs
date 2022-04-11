using System.Net.Http;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<AuthenticationResult> PostLoginAsync(UserLogin userLogin);
}