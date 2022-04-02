using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class LockedMeetingException : Xeption
{
    public LockedMeetingException(Exception innerException)
    : base(message: "Locked meeting record exception, please try again later!", innerException)
    { }
}