using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class FailedMeetingDependencyException : Xeption
{
    public FailedMeetingDependencyException(Exception innerException)
    : base(message:"Failed meeting dependency error occurred, contact support!", innerException)
    { }
}