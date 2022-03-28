using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class MeetingServiceException : Xeption
{
    public MeetingServiceException(Xeption innerException)
        : base(message: "Meeting service error occurred, contact support!", innerException)
    { }
}