using System.Collections;
using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class InvalidUserException : Xeption
{
    public InvalidUserException()
        : base(message: "Fehlerhafter Benutzer. Bitte korrigieren Sie die Fehler und versuchen Sie es erneut!")
    { }

    public InvalidUserException(Exception innerException, IDictionary data)
        : base(message: "Fehlerhafter Benutzer. Bitte korrigieren Sie die Fehler und versuchen Sie es erneut!",
            innerException,
            data)
    { }
}

