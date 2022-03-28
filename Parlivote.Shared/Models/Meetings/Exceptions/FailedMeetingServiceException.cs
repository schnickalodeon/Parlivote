using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class FailedMeetingServiceException : Xeption
{
    public FailedMeetingServiceException(Exception innerException)
    : base(message:"Failed meeting service error occurred, contact support!", innerException)
    { }
}