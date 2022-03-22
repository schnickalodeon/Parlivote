using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class PollServiceException : Xeption
{
    public PollServiceException(Xeption innerException)
        : base(message: "Poll service error occurred, contact support!", innerException)
    { }
}