using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class InvalidUserIdException : Xeption
{
    public InvalidUserIdException()
    : base("The id is in invalid format!")
    {
        
    }
}