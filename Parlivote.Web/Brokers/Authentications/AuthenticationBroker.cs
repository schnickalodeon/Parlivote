using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Parlivote.Shared.Extensions;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Brokers.LocalStorage;
using Parlivote.Web.Configurations;
using Parlivote.Web.Services.Authentication;
using RESTFulSense.Clients;
using RESTFulSense.Exceptions;

namespace Parlivote.Web.Brokers.Authentications;

public class AuthenticationBroker : IAuthenticationBroker
{
    private readonly IRESTFulApiFactoryClient apiClient;
    private readonly HttpClient httpClient;
    private readonly ILocalStorageBroker localStorageBroker;
    private readonly LocalConfigurations localConfigurations;
    private readonly AuthenticationStateProvider authStateProvider;
    public AuthenticationBroker(
        HttpClient httpClient,
        IConfiguration configuration,
        ILocalStorageBroker localStorageBroker, AuthenticationStateProvider authStateProvider)
    {
        this.httpClient = httpClient;
        this.localStorageBroker = localStorageBroker;
        this.authStateProvider = authStateProvider;
        this.localConfigurations = configuration.Get<LocalConfigurations>();
        this.apiClient = GetApiClient();
    }

    private const string IdentityRelativeUrl = "/api/v1/identity";
    public async Task<AuthenticationResult> PostLoginAsync(UserLogin userLogin) =>
        await this.apiClient.PostContentAsync<UserLogin, AuthenticationResult>($"{IdentityRelativeUrl}/login", userLogin);

    public async Task<AuthenticationResult> PostRegisterAsync(UserRegistration registration) =>
        await this.apiClient.PostContentAsync<UserRegistration, AuthenticationResult>($"{IdentityRelativeUrl}/register", registration);

    public async Task<AuthenticationResult> PostRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest) =>
        await this.apiClient.PostContentAsync<RefreshTokenRequest, AuthenticationResult>($"{IdentityRelativeUrl}/refresh", refreshTokenRequest);

    private AuthFailedResponse DeserializeMessage(string message)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<AuthFailedResponse>(message, options);

    }

    private async Task SaveJwtToLocalStorage(AuthenticationResult authResult)
    {
        await this.localStorageBroker.SetTokenAsync(authResult.Token);
        await this.localStorageBroker.SetTokenExpirationAsync(authResult.Token_Expiration);
        await this.localStorageBroker.SetRefreshTokenAsync(authResult.RefreshToken);
    }

    public async Task<AuthenticationHeaderValue> GetAuthorizationHeaderAsync()
    {
        DateTime jwtExpiration =
            await this.localStorageBroker.GetTokenExpirationAsync();

        if (jwtExpiration.IsBefore(DateTime.UtcNow))
        {
            AuthenticationHeaderValue authenticationHeaderValue = await RefreshToken();
            return authenticationHeaderValue;
        }

        var token = await this.localStorageBroker.GetTokenAsync();
        return new AuthenticationHeaderValue("bearer", token);
    }

    private async Task<AuthenticationHeaderValue> RefreshToken()
    {
        try
        {
            AuthenticationResult result = await GetNewTokenFromApi();

            if (result.Success)
            {
                await this.localStorageBroker.SetTokenAsync(result.Token);
                await this.localStorageBroker.SetTokenExpirationAsync(result.Token_Expiration);
                await this.localStorageBroker.SetRefreshTokenAsync(result.RefreshToken);
               return new AuthenticationHeaderValue("bearer", result.Token);

            }
            else
            {
                return new AuthenticationHeaderValue("bearer", "");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return new AuthenticationHeaderValue("bearer", "");
        }
    }

    private async Task<AuthenticationResult> GetNewTokenFromApi()
    {
        try
        {
            string token = await this.localStorageBroker.GetTokenAsync();
            string refreshToken = await this.localStorageBroker.GetRefreshTokenAsync();

            var request = new RefreshTokenRequest
            {
                Token = token,
                RefreshToken = refreshToken
            };

            AuthenticationResult authResult = await PostRefreshTokenAsync(request);

            if (authResult.Success == false)
            {
                return authResult;
            }

            await SaveJwtToLocalStorage(authResult);

            ((AuthStateProvider)this.authStateProvider).NotifyUserAuthentication(authResult.Token);

            return authResult;
        }
        catch (HttpResponseBadRequestException badRequestException)
        {
            return DeserializeMessage(badRequestException.Message);
        }
    }

    private IRESTFulApiFactoryClient GetApiClient()
    {
        string apiBaseUrl = this.localConfigurations.ApiConfigurations.Url;
        this.httpClient.BaseAddress = new Uri(apiBaseUrl);

        return new RESTFulApiFactoryClient(this.httpClient);
    }
}