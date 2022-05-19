using Xeptions;

namespace Parlivote.Shared.Models.Identity.Users.Exceptions;
public class NullUserException : Xeption
{
    public NullUserException()
    : base(message:"Der Benutzer darf nicht null sein!")
    { }
}
