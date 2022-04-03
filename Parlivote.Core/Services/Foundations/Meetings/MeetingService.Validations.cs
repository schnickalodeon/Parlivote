using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;

namespace Parlivote.Core.Services.Foundations.Meetings;

public partial class MeetingService
{
    private void ValidateMeeting(Meeting meeting)
    {
        ValidateMeetingIsNotNull(meeting);

        Validate(
            (IsInvalid(meeting.Id), nameof(Meeting.Id)),
            (IsInvalid(meeting.Description), nameof(Meeting.Description))
            );
    }

    private void ValidateStorageMeeting(Meeting maybeMeeting, Guid meetingId)
    {
        if (maybeMeeting is null)
        {
            throw new NotFoundMeetingException(meetingId);
        }
    }

    private void ValidateMeetingIsNotNull(Meeting meeting)
    {
        if (meeting is null)
        {
            throw new NullMeetingException();
        }
    }

    private void ValidateMeetingId(Guid meetingId)
    {
        Validate((IsInvalid(meetingId), nameof(Meeting.Id)));
    }
    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = ExceptionMessages.INVALID_ID
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = String.IsNullOrWhiteSpace(text),
        Message = ExceptionMessages.INVALID_STRING
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPostException = new InvalidMeetingException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidPostException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidPostException.ThrowIfContainsErrors();
    }
}