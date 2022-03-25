using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class FailedMotionServiceException : Xeption
{
    public FailedMotionServiceException(Exception innerException)
    : base(message:"Failed poll service error occurred, contact support!", innerException)
    { }
}