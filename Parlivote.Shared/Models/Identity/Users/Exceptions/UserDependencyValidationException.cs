using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class UserDependencyValidationException : Xeption
{
    public UserDependencyValidationException(Xeption innerException)
        : base(message: "Beim Verarbeiten des Benutzers ist ein Fehler aufgetreten, bitte versuchen Sie es erneut!",
            innerException)
    { }
}
