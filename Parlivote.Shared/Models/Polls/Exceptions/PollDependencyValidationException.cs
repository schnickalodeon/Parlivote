using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class PollDependencyValidationException : Xeption
{
    public PollDependencyValidationException(Exception innerException)
    : base(message:"Poll dependency validation error occurred, contact support!", innerException)
    { }
}