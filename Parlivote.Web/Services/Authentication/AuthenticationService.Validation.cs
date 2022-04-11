using System;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Exceptions;
using Parlivote.Shared.Models.Meetings.Exceptions;

namespace Parlivote.Web.Services.Authentication;

public partial class AuthenticationService
{
    private void ValidateRegistration(UserRegistration registration)
    {
        ValidateEmailsAreIdentical(registration.Email, registration.EmailConfirmation);

        Validate(
            (IsEmailInvalid(registration.Email), nameof(UserRegistration.Email)),
            (IsEmailInvalid(registration.EmailConfirmation), nameof(UserRegistration.EmailConfirmation)),
            (IsInvalid(registration.Password), nameof(UserRegistration.Password))
            );
    }

    private void ValidateLogin(UserLogin login)
    {
        Validate(
            (IsEmailInvalid(login.Email), nameof(UserLogin.Email)),
            (IsInvalid(login.Password), nameof(UserLogin.Password))
        );
    }

    private static bool HasEmailInvalidFormat(string email)
    {
        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                   + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                   + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        return !Regex.IsMatch(email, validEmailPattern, RegexOptions.IgnoreCase);
    }

    private void ValidateEmailsAreIdentical(string email, string emailConfirmation)
    {
        if (email != emailConfirmation)
        {
            throw new EmailNotConfirmedException();
        }
    }

    private static dynamic IsEmailInvalid(string email) => new
    {
        Condition = string.IsNullOrWhiteSpace(email) || HasEmailInvalidFormat(email),
        Message = ExceptionMessages.INVALID_STRING
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = string.IsNullOrWhiteSpace(text),
        Message = ExceptionMessages.INVALID_STRING
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPostException = new InvalidAuthenticationException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidPostException.AddData(
                    key: parameter,
                    values: rule.Message);
            }
        }

        invalidPostException.ThrowIfContainsErrors();
    }

   
}