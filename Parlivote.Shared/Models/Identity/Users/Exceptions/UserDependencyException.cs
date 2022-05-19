using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;

public class UserDependencyException : Xeption
{
    public UserDependencyException(Xeption innerException)
        : base(message: "Ein Fehler bei der Verarbeitung des Benutzers ist aufgetreten, bitte kontaktieren Sie die DOOR GmbH", innerException)
    { }
}
