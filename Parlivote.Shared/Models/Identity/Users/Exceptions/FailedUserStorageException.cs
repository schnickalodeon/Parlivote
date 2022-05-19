using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class FailedUserStorageException : Xeption
{
    public FailedUserStorageException(Exception innerException)
        : base(message: "Ein Fehler beim Zugriff auf die Datenbank ist aufgetreten, Bitte kontaktieren Sie die DOOR GmbH", innerException)
    { }

    
}
