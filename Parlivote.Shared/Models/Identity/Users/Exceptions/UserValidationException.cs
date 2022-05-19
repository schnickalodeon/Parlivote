using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
    public class UserValidationException : Xeption
    {
        public UserValidationException(Xeption innerException)
            : base(message: "Ein Fehler bei der Validierung des Benutzers ist aufgetreten, bitte versuchen Sie es erneut",
                innerException)
        { }
    }
