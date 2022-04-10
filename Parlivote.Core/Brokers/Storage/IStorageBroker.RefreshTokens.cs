using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    Task<RefreshToken> SelectRefreshTokenByToken(string token);
    Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken>InsertRefreshTokenAsync(RefreshToken token);
}