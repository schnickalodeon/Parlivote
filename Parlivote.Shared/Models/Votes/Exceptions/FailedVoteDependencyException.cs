using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class FailedVoteDependencyException : Xeption
{
    public FailedVoteDependencyException(Exception innerException)
    : base(message:"Failed poll dependency error occurred, contact support!", innerException)
    { }
}