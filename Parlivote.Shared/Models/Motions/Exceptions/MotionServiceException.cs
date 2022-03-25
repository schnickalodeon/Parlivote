using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class MotionServiceException : Xeption
{
    public MotionServiceException(Xeption innerException)
        : base(message: "Motion service error occurred, contact support!", innerException)
    { }
}