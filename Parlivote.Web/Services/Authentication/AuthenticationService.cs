using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Authentications;
using Parlivote.Web.Brokers.LocalStorage;
using Parlivote.Web.Configurations;
using RESTFulSense.Exceptions;

namespace Parlivote.Web.Services.Authentication
{
    public partial class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationBroker authenticationBroker;
        private readonly ILocalStorageBroker localStorageBroker;
        private readonly AuthenticationStateProvider authStateProvider;

        public AuthenticationService(
            AuthenticationStateProvider authStateProvider,
            IAuthenticationBroker authenticationBroker, 
            ILocalStorageBroker localStorageBroker)
        {
            this.authenticationBroker = authenticationBroker;
            this.localStorageBroker = localStorageBroker;
            this.authStateProvider = authStateProvider;
        }

        public async Task<AuthenticationResult> LoginAsync(UserLogin login)
        {
            try
            {
                ValidateLogin(login);

                AuthenticationResult authResult = await this.authenticationBroker.PostLoginAsync(login);

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
        public async Task<AuthenticationResult> RegisterAsync(UserRegistration registration)
        {
            try
            {
                ValidateRegistration(registration);

                AuthenticationResult authenticationResult =
                    await this.authenticationBroker.PostRegisterAsync(registration);

                if (authenticationResult.Success == false)
                {
                    return authenticationResult;
                }

                return new AuthSuccessResponse(
                    authenticationResult.Token, 
                    authenticationResult.Token_Expiration,
                    authenticationResult.RefreshToken);
            }
            catch (HttpResponseBadRequestException badRequestException)
            {
                return DeserializeMessage(badRequestException.Message);
            }
        }

        private AuthFailedResponse DeserializeMessage(string message)
        {
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            return JsonSerializer.Deserialize<AuthFailedResponse>(message, options);

        }
        private async Task SaveJwtToLocalStorage(AuthenticationResult authResult)
        {
            await this.localStorageBroker.SetTokenAsync(authResult.Token);
            await this.localStorageBroker.SetTokenExpirationAsync(authResult.Token_Expiration);
            await this.localStorageBroker.SetRefreshTokenAsync(authResult.RefreshToken);
        }
        private async Task DeleteJwtStorageData()
        {
            await this.localStorageBroker.DeleteTokenAsync();
            await this.localStorageBroker.DeleteTokenExpirationAsync();
            await this.localStorageBroker.DeleteRefreshTokenAsync();
        }
        public async Task LogoutAsync()
        {
            await DeleteJwtStorageData();
            ((AuthStateProvider)this.authStateProvider).NotifyUserLogout();
        }

        //public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        //{
        //    var host = _configuration["uris:web-api-host"];
        //    var url = host + ApiRoutes.Identity.ChangePassword;
        //    var response = await _httpClient.PostAsJsonAsync(url, request);

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    var result = JsonSerializer.Deserialize<ChangePasswordResponse>(responseContent,
        //       new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    return result;

        //}

        //public async Task<ApiResponse> ConfirmEmail(string token, string userId)
        //{
        //    var host = _configuration["uris:web-api-host"];

        //    var encodedToken = HttpUtility.UrlEncode(token);

        //    var request = new ConfirmEmailRequest
        //    {
        //        UserId = userId,
        //        Token = encodedToken
        //    };

        //    var url = host + ApiRoutes.Identity.ConfirmMail;

        //    var responseMessage = await _httpClient.PostAsJsonAsync(url, request);
        //    var content = await responseMessage.Content.ReadAsStringAsync();

        //    var result = JsonSerializer.Deserialize<ApiResponse>(content,
        //      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    return result;          
        //}

    }
}
