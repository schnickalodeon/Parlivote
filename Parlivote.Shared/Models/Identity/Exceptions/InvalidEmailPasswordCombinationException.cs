using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class InvalidEmailPasswordCombinationException : Xeption
{
    public InvalidEmailPasswordCombinationException()
        : base("User/Password combination is wrong"){ }
}