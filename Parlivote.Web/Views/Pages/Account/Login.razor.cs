using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Identity;
using Parlivote.Web.Models;
using Parlivote.Web.Services.Authentication;

namespace Parlivote.Web.Views.Pages.Account
{
    public partial class Login
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private readonly UserLogin login = new();
        private ComponentState state = ComponentState.Content;
        private string error;
        private bool loginSuccessful = false;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                this.state = ComponentState.Content;
            }
        }

        private async void LoginAsync()
        {
            try
            {
                this.state = ComponentState.Loading;
                await this.AuthenticationService.LoginAsync(this.login);
                this.loginSuccessful = true;
                this.state = ComponentState.Content;
                await InvokeAsync(StateHasChanged);
                NavigationManager.NavigateTo("/");
            }
            catch (Exception e)
            {
                this.error = e.Message;
                this.state = ComponentState.Error;
            }
        }
    }
}
