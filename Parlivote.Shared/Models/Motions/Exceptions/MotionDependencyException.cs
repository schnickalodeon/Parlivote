using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class MotionDependencyException : Xeption
{
    public MotionDependencyException(Xeption innerException)
        : base(message: "Motion dependency error occurred, contact support.", innerException)
    { }
}