using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;

public class AlreadyExistsUserException : Xeption
{
    public AlreadyExistsUserException(Exception innerException)
        : base(message: "Ein Benutzer mit dieser Id existiert bereits!", innerException)
    {
    }
}