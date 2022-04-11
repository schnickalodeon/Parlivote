using Microsoft.Extensions.Configuration;
using Parlivote.Web.Brokers.LocalStorage;
using Parlivote.Web.Configurations;
using Parlivote.Web.Services.Authentication;
using RESTFulSense.Clients;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Parlivote.Web.Brokers.Authentications;

namespace Parlivote.Web.Brokers.API
{
    public partial class ApiBroker : IApiBroker
    {
        private readonly IRESTFulApiFactoryClient apiClient;
        private readonly IAuthenticationBroker authenticationBroker;
        private readonly HttpClient httpClient;

        public ApiBroker(
            HttpClient httpClient,
            IConfiguration configuration, 
            IAuthenticationBroker authenticationBroker)
        {
            this.httpClient = httpClient;
            this.authenticationBroker = authenticationBroker;
            this.apiClient = GetApiClient(configuration);
        }

        private async ValueTask<T> PostAsync<T>(string relativeUrl, T content)
        {
            await SetAuthorization();
            return await this.apiClient.PostContentAsync<T>(relativeUrl, content);
        }

        private async ValueTask<U> PostAsync<T, U>(string relativeUrl, T content)
        {
            await SetAuthorization();
            return await this.apiClient.PostContentAsync<T, U>(relativeUrl, content);
        }

        private async ValueTask<T> GetAsync<T>(string relativeUrl)
        {
            await SetAuthorization();
            return await this.apiClient.GetContentAsync<T>(relativeUrl);
        }

        private async ValueTask<T> PutAsync<T>(string relativeUrl, T content)
        {
            await SetAuthorization();
            return await this.apiClient.PutContentAsync<T>(relativeUrl, content);
        }

        private async ValueTask<T> DeleteAsync<T>(string relativeUrl)
        {
            await SetAuthorization();
            return await this.apiClient.DeleteContentAsync<T>(relativeUrl);
        }

        private async Task SetAuthorization()
        {

            AuthenticationHeaderValue authenticationHeaderValue =
                await this.authenticationBroker.GetAuthorizationHeaderAsync();

            this.httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        private IRESTFulApiFactoryClient GetApiClient(IConfiguration configuration)
        {
            LocalConfigurations localConfigurations =
                configuration.Get<LocalConfigurations>();

            string apiBaseUrl = localConfigurations.ApiConfigurations.Url;
            this.httpClient.BaseAddress = new Uri(apiBaseUrl);

            return new RESTFulApiFactoryClient(this.httpClient);
        }
    }
}
