using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class MotionValidationException : Xeption
{
    public MotionValidationException(Xeption innerException)
    : base(message:"Motion validation error occurred, please try again", innerException)
    { }
}