using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class FailedMeetingStorageException : Xeption
{
    public FailedMeetingStorageException(Exception innerException)
        : base(message:"Database error occurred, contact support!", innerException)
    { }
}