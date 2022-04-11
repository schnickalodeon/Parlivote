using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Parlivote.Web.Configurations;

namespace Parlivote.Web.Brokers.LocalStorage;

public partial class LocalStorageBroker : ILocalStorageBroker
{
    private readonly ProtectedLocalStorage localStorage;
    private readonly LocalConfigurations localConfigurations;

    public LocalStorageBroker(ProtectedLocalStorage localStorage, IConfiguration configuration)
    {
        this.localStorage = localStorage;
        this.localConfigurations = configuration.Get<LocalConfigurations>();
    }
    public async Task<T> GetByKeyAsync<T>(string key)
    {
        ProtectedBrowserStorageResult<T> result =
            await this.localStorage.GetAsync<T>(key);

        return result.Success ? result.Value : throw new Exception($"Error at reading {key} from local storage!");
    }

    public async Task SetAsync(string key, object value)
    {
        await this.localStorage.SetAsync(key, value);
    }

    public async Task Delete(string key)
    {
        await this.localStorage.DeleteAsync(key);
    }
}