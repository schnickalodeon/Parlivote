using Xeptions;

namespace Parlivote.Shared.Models.Polls.Exceptions;

public class AlreadyExistsPollException : Xeption
{
    public AlreadyExistsPollException(Exception innerException)
        : base(message:"There is already a poll with the given Id!", innerException)
    { }
}