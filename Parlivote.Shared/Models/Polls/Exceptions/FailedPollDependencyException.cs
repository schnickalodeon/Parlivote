using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class FailedPollDependencyException : Xeption
{
    public FailedPollDependencyException(Exception innerException)
    : base(message:"Failed poll dependency error occurred, contact support!", innerException)
    { }
}