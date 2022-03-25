using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class FailedMotionDependencyException : Xeption
{
    public FailedMotionDependencyException(Exception innerException)
    : base(message:"Failed poll dependency error occurred, contact support!", innerException)
    { }
}