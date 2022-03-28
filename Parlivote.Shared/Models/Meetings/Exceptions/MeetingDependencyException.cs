using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class MeetingDependencyException : Xeption
{
    public MeetingDependencyException(Xeption innerException)
        : base(message: "Meeting dependency error occurred, contact support.", innerException)
    { }
}