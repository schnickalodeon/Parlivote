using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class UserRegistrationException : Xeption
{
    public IEnumerable<string> Errors { get; set; }
    public UserRegistrationException(IEnumerable<string> errorMessages)
    : base("Something went wrong at the registration!")
    {
        this.Errors = errorMessages;
    }
}