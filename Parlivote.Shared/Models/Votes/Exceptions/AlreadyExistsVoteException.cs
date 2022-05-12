using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class AlreadyExistsVoteException : Xeption
{
    public AlreadyExistsVoteException(Exception innerException)
        : base(message:"There is already a poll with the given Id!", innerException)
    { }
}