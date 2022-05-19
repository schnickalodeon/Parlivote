using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class FailedUserDependencyException : Xeption
{
    public FailedUserDependencyException(Exception innerException)
        : base(message: "Auf dem Server ist ein Fehler beim Verarbeiten des Benutzers aufgetreten, bitte kontaktieren Sie den Support",
            innerException)
    { } 
}
