using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class AlreadyExistsMotionException : Xeption
{
    public AlreadyExistsMotionException(Exception innerException)
        : base(message:"There is already a poll with the given Id!", innerException)
    { }
}