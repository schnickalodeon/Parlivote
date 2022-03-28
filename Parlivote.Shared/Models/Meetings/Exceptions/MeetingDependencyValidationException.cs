using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class MeetingDependencyValidationException : Xeption
{
    public MeetingDependencyValidationException(Exception innerException)
    : base(message:"Meeting dependency validation error occurred, contact support!", innerException)
    { }
}