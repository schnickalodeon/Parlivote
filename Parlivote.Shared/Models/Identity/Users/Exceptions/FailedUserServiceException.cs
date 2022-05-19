using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class FailedUserServiceException : Xeption
{
    public FailedUserServiceException(Exception innerException)
        : base(message: "Fehler beim Verarbeiten des Benutzers, bitte kontaktieren Sie die DOOR GmbH!", innerException)
    {

    }
}
