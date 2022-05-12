using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class InvalidVoteException : Xeption
{
    public InvalidVoteException()
    : base("Invalid Vote, please correct the errors and try again!")
    { }
}