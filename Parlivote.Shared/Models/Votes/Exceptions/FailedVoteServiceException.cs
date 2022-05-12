using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class FailedVoteServiceException : Xeption
{
    public FailedVoteServiceException(Exception innerException)
    : base(message:"Failed poll service error occurred, contact support!", innerException)
    { }
}