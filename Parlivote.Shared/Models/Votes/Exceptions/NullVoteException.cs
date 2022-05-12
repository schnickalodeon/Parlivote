using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class NullVoteException : Xeption
{
    public NullVoteException()
    : base(message:"Vote cannot be empty!")
    { }
}