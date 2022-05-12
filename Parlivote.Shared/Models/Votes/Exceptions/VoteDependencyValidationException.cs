using Xeptions;

namespace Parlivote.Shared.Models.Votes.Exceptions;

public class VoteDependencyValidationException : Xeption
{
    public VoteDependencyValidationException(Exception innerException)
    : base(message:"Vote dependency validation error occurred, contact support!", innerException)
    { }
}