using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class LockedVoteException : Xeption
{
    public LockedVoteException(Exception innerException)
    : base(message: "Locked vote record exception, please try again later!", innerException)
    { }
}