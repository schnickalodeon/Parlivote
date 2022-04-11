using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Exceptions;
using Parlivote.Web.Models;
using Parlivote.Web.Services.Authentication;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Parlivote.Web.Views.Pages.Account
{
    public partial class Register
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private readonly UserRegistration registration = new();
        private ComponentState state = ComponentState.Content;
        private string error = "";
        private bool registrationSuccessul = false;
        private IDictionary validationErrors;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                this.state = ComponentState.Content;
            }
        }

        private async Task LoginAsync()
        {
            try
            {
                this.validationErrors = null;
                this.state = ComponentState.Loading;
                var login = new UserLogin(this.registration.Email, this.registration.Password);

                AuthenticationResult result =
                    await this.AuthenticationService.LoginAsync(login);

                this.state = ComponentState.Content;
                if (result.Success)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    this.error = string.Join(",", result.ErrorMessages);
                    this.state = ComponentState.Error;
                }

            }
            catch (Exception e)
            {
                this.error = e.Message;
                this.state = ComponentState.Error;
            }
        }

        private async void RegisterAsync()
        {
            try
            {
                this.validationErrors = null;

                this.state = ComponentState.Loading;

                AuthenticationResult result =
                    await this.AuthenticationService.RegisterAsync(this.registration);

                this.state = ComponentState.Content;
                if (result.Success)
                {
                    this.registrationSuccessul = true;
                    await InvokeAsync(StateHasChanged);
                    await LoginAsync();
                }
                else
                {
                    this.error = String.Join(",", result.ErrorMessages);
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
