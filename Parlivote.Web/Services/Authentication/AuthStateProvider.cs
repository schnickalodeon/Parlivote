using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Parlivote.Web.Brokers.API;
using Syncfusion.Blazor.Kanban.Internal;

namespace Parlivote.Web.Services.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly ProtectedLocalStorage localStorage;
        private readonly AuthenticationState anonymous;
        private readonly string authTokenStorageKey;
        private readonly IApiBroker apiBroker;

        public AuthStateProvider(HttpClient httpClient, IConfiguration configuration, ProtectedLocalStorage localStorage, IApiBroker apiBroker)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.apiBroker = apiBroker;
            this.anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            this.authTokenStorageKey = "authToken";
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ProtectedBrowserStorageResult<string> tokenResult = 
                await this.localStorage.GetAsync<string>(this.authTokenStorageKey);

            string token = tokenResult.Value;

            if (string.IsNullOrWhiteSpace(token))
            {
                return this.anonymous;
            }

            var claims = JwtParser.ParseClaimsFromJwt(token);

            var claimsIdentity = new ClaimsIdentity(claims, "jwtAuthType", "name", "role");

            return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token);

            var claimsIdentity = new ClaimsIdentity(claims, "jwtAuthType", "name", "role");

            var authenticatedUser = new ClaimsPrincipal(claimsIdentity);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(this.anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
