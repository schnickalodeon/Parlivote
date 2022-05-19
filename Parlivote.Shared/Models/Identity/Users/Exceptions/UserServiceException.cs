using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class UserServiceException : Xeption
{
    public UserServiceException(Xeption innerException)
    : base(message: "Ein Fehler beim Verarbeiten des Benutzers ist aufgetreten, bitte kontaktieren Sie DOOR",
        innerException)
    {

    }
}
