using System;
using System.Threading.Tasks;

namespace Parlivote.Web.Brokers.LocalStorage;

public partial class LocalStorageBroker
{
    public async Task<string> GetTokenAsync()
    {
        string key = this.localConfigurations.AuthTokenStorageKey;
        return await GetByKeyAsync<string>(key);
    }

    public async Task SetTokenAsync(string token)
    {
        string key = this.localConfigurations.AuthTokenStorageKey;
        await SetAsync(key, token);
    }

    public async Task DeleteTokenAsync()
    {
        string key = this.localConfigurations.AuthTokenStorageKey;
        await Delete(key);
    }

    public Task<DateTime> GetTokenExpirationAsync()
    {
        string key = this.localConfigurations.AuthTokenExpirationStorageKey;
        return GetByKeyAsync<DateTime>(key);
    }

    public async Task SetTokenExpirationAsync(DateTime tokenExpiration)
    {
        string key = this.localConfigurations.AuthTokenExpirationStorageKey;
        await SetAsync(key, tokenExpiration);
    }

    public async Task DeleteTokenExpirationAsync()
    {
        string key = this.localConfigurations.AuthTokenExpirationStorageKey;
        await Delete(key);
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        string key = this.localConfigurations.RefreshTokenStorageKey;
        return  await GetByKeyAsync<string>(key);
    }

    public async Task SetRefreshTokenAsync(string refreshToken)
    {
        string key = this.localConfigurations.RefreshTokenStorageKey;
        await SetAsync(key, refreshToken);
    }

    public async Task DeleteRefreshTokenAsync()
    {
        string key = this.localConfigurations.RefreshTokenStorageKey;
        await Delete(key);
    }
}