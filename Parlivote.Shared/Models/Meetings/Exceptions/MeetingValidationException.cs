using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class MeetingValidationException : Xeption
{
    public MeetingValidationException(Xeption innerException)
    : base(message:"Meeting validation error occurred, please try again", innerException)
    { }
}