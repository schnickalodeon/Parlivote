using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class NotFoundMeetingException : Xeption
{
    public NotFoundMeetingException(Guid meetingId)
    : base(message: $"Could not find meeting with id: {meetingId}")
    {
        
    }
}