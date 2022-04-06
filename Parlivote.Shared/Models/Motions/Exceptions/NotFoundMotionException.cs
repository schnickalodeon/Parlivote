using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class NotFoundMotionException : Xeption
{
    public NotFoundMotionException(Guid motionId)
    : base(message: $"Could not find motion with id: {motionId}")
    {
        
    }
}