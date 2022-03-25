using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class FailedPollServiceException : Xeption
{
    public FailedPollServiceException(Exception innerException)
    : base(message:"Failed poll service error occurred, contact support!", innerException)
    { }
}