using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class MotionDependencyValidationException : Xeption
{
    public MotionDependencyValidationException(Exception innerException)
    : base(message:"Motion dependency validation error occurred, contact support!", innerException)
    { }
}