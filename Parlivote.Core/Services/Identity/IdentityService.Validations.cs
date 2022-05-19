using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Exceptions;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Services.Identity;

public partial class IdentityService
{
    private void ValidateStorageUser(User user)
    {
        if (user is null)
        {
            throw new InvalidEmailPasswordCombinationException();
        }
    }

    private void ValidateEmailAddress(string email)
    {

        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                   + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                   + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        if (!Regex.IsMatch(email,validEmailPattern,RegexOptions.IgnoreCase))
        {
            throw new InvalidEmailException();
        }

    }

    private async Task ValidatePassword(User user, string password)
    {
        bool userHasValidPassword =
            await this.userManager.CheckPasswordAsync(user, password);

        if (!userHasValidPassword)
        {
            throw new InvalidEmailPasswordCombinationException();
        }
    }

    private async Task ValidateUserDoesNotExist(string email)
    {
        User existingUser =
            await this.userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException();
        }
    }

    private void ValidateCreatedUser(IdentityResult createdUserResult)
    {
        if (!createdUserResult.Succeeded)
        {
            IEnumerable<string> errors = createdUserResult.Errors.Select(x => x.Description);
            throw new UserRegistrationException(errors);
        }
    }
}