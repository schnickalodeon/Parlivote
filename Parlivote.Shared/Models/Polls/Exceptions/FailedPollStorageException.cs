using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class FailedPollStorageException : Xeption
{
    public FailedPollStorageException(Exception innerException)
        : base(message:"Database error occurred, contact support!", innerException)
    { }
}