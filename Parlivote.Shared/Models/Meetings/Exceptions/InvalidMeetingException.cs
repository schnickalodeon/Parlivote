using Xeptions;

namespace Parlivote.Shared.Models.Meetings.Exceptions;

public class InvalidMeetingException : Xeption
{
    public InvalidMeetingException()
    : base("Invalid Meeting, please correct the errors and try again!")
    { }
}