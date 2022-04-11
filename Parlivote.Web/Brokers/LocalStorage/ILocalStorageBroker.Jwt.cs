using System;
using System.Threading.Tasks;

namespace Parlivote.Web.Brokers.LocalStorage;

public partial interface ILocalStorageBroker
{
    Task<string> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task DeleteTokenAsync();
    Task<DateTime> GetTokenExpirationAsync();
    Task SetTokenExpirationAsync(DateTime tokenExpiration);
    Task DeleteTokenExpirationAsync();
    Task<string> GetRefreshTokenAsync();
    Task SetRefreshTokenAsync(string refreshToken);
    Task DeleteRefreshTokenAsync();

}