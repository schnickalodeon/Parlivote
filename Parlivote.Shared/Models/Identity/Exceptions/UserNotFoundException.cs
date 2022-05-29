using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class UserNotFoundException : Xeption
{
    public UserNotFoundException()
    : base("There is no user with the given Id!")
    {
        
    }
}