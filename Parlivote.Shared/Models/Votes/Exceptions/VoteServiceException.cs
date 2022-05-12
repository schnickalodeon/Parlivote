using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class VoteServiceException : Xeption
{
    public VoteServiceException(Xeption innerException)
        : base(message: "Vote service error occurred, contact support!", innerException)
    { }
}