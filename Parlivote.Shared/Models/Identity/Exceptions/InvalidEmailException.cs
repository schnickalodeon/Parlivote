using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class InvalidEmailException : Xeption
{
    public InvalidEmailException()
    : base("The entered e-mail address is in invalid format!")
    {
        
    }
}