using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Exceptions;
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
        private IDictionary validationErrors;
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
                this.validationErrors = null;
                this.state = ComponentState.Loading;

                AuthenticationResult result =
                    await this.AuthenticationService.LoginAsync(this.login);

                this.state = ComponentState.Content;
                if (result.Success)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    this.error = string.Join(",", result.ErrorMessages);
                }

            }
            catch (EmailNotConfirmedException emailNotConfirmedException)
            {
                this.error = emailNotConfirmedException.Message;

            }
            catch (InvalidAuthenticationException invalidAuthenticationException)
            {
                this.error = invalidAuthenticationException.Message;
                this.validationErrors = invalidAuthenticationException.Data;
            }
            this.state = ComponentState.Content;
            await InvokeAsync(StateHasChanged);
        }
    }
}
