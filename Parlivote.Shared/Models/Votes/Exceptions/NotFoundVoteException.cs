using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class NotFoundVoteException : Xeption
{
    public NotFoundVoteException(Guid voteId)
    : base(message: $"Could not find vote with id: {voteId}")
    {
        
    }
}