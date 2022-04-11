using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Brokers.API;

namespace Parlivote.Web.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient httpClient;
        private readonly IApiBroker apiBroker;
        private readonly ProtectedLocalStorage localStorage;
        private readonly AuthenticationStateProvider authStateProvider;
        private readonly string authTokenStorageKey = "authToken";

        public AuthenticationService(
            HttpClient httpClient, 
            AuthenticationStateProvider authStateProvider, 
            IApiBroker apiBroker, ProtectedLocalStorage localStorage)
        {
            this.httpClient = httpClient;
            this.apiBroker = apiBroker;
            this.localStorage = localStorage;
            this.authStateProvider = authStateProvider;
        }

        public async Task<AuthenticationResult> LoginAsync(UserLogin userForAuth)
        {
            AuthenticationResult authResult = await this.apiBroker.PostLoginAsync(userForAuth);

            if (authResult.Success == false)
            {
                return authResult;
            }

            await this.localStorage.SetAsync(this.authTokenStorageKey, authResult.Token);

            ((AuthStateProvider) this.authStateProvider).NotifyUserAuthentication(authResult.Token);

            return authResult;
        }

        //public async Task<AuthenticationResult> Register(UserRegistrationRequest user)
        //{
        //    var host = configuration["uris:web-api-host"];
        //    var url = host + ApiRoutes.Identity.Register;
        //    var response = await httpClient.PostAsJsonAsync(url, user);

        //    var respContent = await response.Content.ReadAsStringAsync();

        //    if (response.IsSuccessStatusCode == false)
        //    {
        //        var failedResponse = JsonSerializer.Deserialize<AuthFailedResponse>(respContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //        return new AuthenticationResult
        //        {
        //            Success = false,
        //            ErrorMessages = failedResponse.Errors
        //        };
        //    }

        //    var successResponse = JsonSerializer.Deserialize<AuthSuccessResponse>(respContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    return new AuthenticationResult
        //    {
        //        Success = true,
        //        ErrorMessages = null,
        //        UserName = successResponse.UserName,
        //        Access_Token = successResponse.Access_Token
        //    };

        //}

        public async Task LogoutAsync()
        {
            await this.localStorage.DeleteAsync(this.authTokenStorageKey);
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
