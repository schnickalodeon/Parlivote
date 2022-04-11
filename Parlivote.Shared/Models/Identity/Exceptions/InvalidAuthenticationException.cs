using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class InvalidAuthenticationException : Xeption
{
    public InvalidAuthenticationException()
        : base(message: "The entered credentials are invalid, please try again")
    {
    }

    
}