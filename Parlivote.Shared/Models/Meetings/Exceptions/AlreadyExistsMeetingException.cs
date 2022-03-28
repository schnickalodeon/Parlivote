using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class AlreadyExistsMeetingException : Xeption
{
    public AlreadyExistsMeetingException(Exception innerException)
        : base(message:"There is already a meeting with the given Id!", innerException)
    { }
}