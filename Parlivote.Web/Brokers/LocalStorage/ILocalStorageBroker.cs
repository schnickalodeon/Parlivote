using System.Threading.Tasks;

namespace Parlivote.Web.Brokers.LocalStorage;

public partial interface ILocalStorageBroker
{
    Task<T> GetByKeyAsync<T>(string key);
    Task SetAsync(string key, object value);
    Task Delete(string key);
}