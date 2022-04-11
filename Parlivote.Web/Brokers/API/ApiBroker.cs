using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Parlivote.Web.Configurations;
using RESTFulSense.Clients;

namespace Parlivote.Web.Brokers.API
{
    public partial class ApiBroker : IApiBroker
    {
        private readonly IRESTFulApiFactoryClient apiClient;
        private readonly HttpClient httpClient;
        private readonly ProtectedLocalStorage localStorage;
        private AuthenticationHeaderValue authHeader;

        public ApiBroker(HttpClient httpClient, IConfiguration configuration, ProtectedLocalStorage localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.apiClient = GetApiClient(configuration);
        }

        private async ValueTask<T> PostAsync<T>(string relativeUrl, T content) => 
            await this.apiClient.PostContentAsync<T>(relativeUrl, content);

        private async ValueTask<U> PostAsync<T, U>(string relativeUrl, T content) =>
            await this.apiClient.PostContentAsync<T,U>(relativeUrl, content);

        private async ValueTask<T> GetAsync<T>(string relativeUrl)
        {
            await SetAuthorization();
            return await this.apiClient.GetContentAsync<T>(relativeUrl);
        }
            
        private async ValueTask<T> PutAsync<T>(string relativeUrl, T content) => 
            await this.apiClient.PutContentAsync<T>(relativeUrl, content);
        private async ValueTask<T> DeleteAsync<T>(string relativeUrl) =>
            await this.apiClient.DeleteContentAsync<T>(relativeUrl);

        private IRESTFulApiFactoryClient GetApiClient(IConfiguration configuration)
        {
            LocalConfigurations localConfigurations =
                configuration.Get<LocalConfigurations>();

            string apiBaseUrl = localConfigurations.ApiConfigurations.Url;
            this.httpClient.BaseAddress = new Uri(apiBaseUrl);

            return new RESTFulApiFactoryClient(this.httpClient);
        }

        public async Task SetAuthorization()
        {
            var localStorageTokenResult =
                await this.localStorage.GetAsync<string>("authToken");

            var token = localStorageTokenResult.Value;

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }
}
