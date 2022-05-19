using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class NotFoundUserException : Xeption
{
    public NotFoundUserException(string guid) 
        : base(message:$"Ein Benutzer mit der Id {guid} wurde nicht gefunden!")
    { }
    public NotFoundUserException(Exception innerException)
        : base(message: innerException.Message, innerException)
    { }
}
