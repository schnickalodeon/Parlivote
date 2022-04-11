using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class UserAlreadyExistsException : Xeption
{
    public UserAlreadyExistsException()
    : base("An user with this email address already exists")
    {
        
    }
}