using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class PollValidationException : Xeption
{
    public PollValidationException(Xeption innerException)
    : base(message:"Poll validation error occurred, please try again")
    { }
}