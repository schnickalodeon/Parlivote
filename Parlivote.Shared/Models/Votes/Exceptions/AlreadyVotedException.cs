using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class AlreadyVotedException : Xeption
{
    public AlreadyVotedException()
    :base(message: "You have already voted for this motion!")
    { }
}