using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class VoteDependencyException : Xeption
{
    public VoteDependencyException(Xeption innerException)
        : base(message: "Vote dependency error occurred, contact support.", innerException)
    { }
}