using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class LockedMotionException : Xeption
{
    public LockedMotionException(Exception innerException)
    : base(message: "Locked motion record exception, please try again later!", innerException)
    { }
}