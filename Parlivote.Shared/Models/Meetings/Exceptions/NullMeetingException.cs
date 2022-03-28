using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class NullMeetingException : Xeption
{
    public NullMeetingException()
    : base(message:"Meeting cannot be empty!")
    { }
}