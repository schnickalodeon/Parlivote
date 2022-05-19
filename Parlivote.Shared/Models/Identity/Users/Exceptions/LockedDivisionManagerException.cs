using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;

public class LockedUserException : Xeption
{
    public LockedUserException(Exception innerException)
        : base(message:"Dieser Benutzer wird gerade von jemand anderem bearbeitet! Versuchen Sie es in ein paar Minuten erneut"
            , innerException)
    { }
}