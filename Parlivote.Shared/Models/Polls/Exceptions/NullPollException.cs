using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class NullPollException : Xeption
{
    public NullPollException()
    : base(message:"Poll cannot be empty!")
    { }
}