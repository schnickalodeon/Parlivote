using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class InvalidPollException : Xeption
{
    public InvalidPollException()
    : base("Invalid Poll, please correct the errors and try again!")
    { }
}