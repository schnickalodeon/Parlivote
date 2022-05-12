using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class FailedVoteStorageException : Xeption
{
    public FailedVoteStorageException(Exception innerException)
        : base(message:"Database error occurred, contact support!", innerException)
    { }
}