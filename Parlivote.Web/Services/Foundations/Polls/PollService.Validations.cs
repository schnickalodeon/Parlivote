﻿using System;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;

namespace Parlivote.Web.Services.Foundations.Polls;

public partial class PollService
{
    private void ValidatePoll(Poll poll)
    {
        ValidatePollIsNotNull(poll);

        Validate(
            (IsInvalid(poll.Id), nameof(Poll.Id)),
            (IsInvalid(poll.Text), nameof(Poll.Text))
        );
    }

    private void ValidatePollIsNotNull(Poll poll)
    {
        if (poll is null)
        {
            throw new NullPollException();
        }
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
        var invalidPostException = new InvalidPollException();

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