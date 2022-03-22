using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class PollDependencyException : Xeption
{
    public PollDependencyException(Xeption innerException)
        : base(message: "Poll dependency error occurred, contact support.", innerException)
    { }
}