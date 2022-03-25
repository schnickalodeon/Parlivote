using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class FailedMotionStorageException : Xeption
{
    public FailedMotionStorageException(Exception innerException)
        : base(message:"Database error occurred, contact support!", innerException)
    { }
}