using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class VoteValidationException : Xeption
{
    public VoteValidationException(Xeption innerException)
    : base(message:"Vote validation error occurred, please try again", innerException)
    { }
}