using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;

namespace Parlivote.Core.Services.Foundations.Motions;

public partial class MotionService
{
    private void ValidateMotion(Motion poll)
    {
        ValidateMotionIsNotNull(poll);

        Validate(
            (IsInvalid(poll.Id), nameof(Motion.Id)),
            (IsInvalid(poll.Text), nameof(Motion.Text)),
            (IsInvalid(poll.Title), nameof(Motion.Title)),
            (IsInvalid(poll.ApplicantId), nameof(Motion.ApplicantId))
            );
    }

    private void ValidateMotionIsNotNull(Motion poll)
    {
        if (poll is null)
        {
            throw new NullMotionException();
        }
    }

    private void ValidateMotionId(Guid motionId)
    {
        Validate((IsInvalid(motionId), nameof(Motion.Id)));
    }
    private void ValidateStorageMotion(Motion maybeMotion, Guid motionId)
    {
        if (maybeMotion is null)
        {
            throw new NotFoundMotionException(motionId);
        }
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = ExceptionMessages.INVALID_ID
    };

    private static dynamic IsInvalid(Guid? id) => new
    {
        Condition = !id.HasValue || id == Guid.Empty,
        Message = ExceptionMessages.INVALID_ID
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = String.IsNullOrWhiteSpace(text),
        Message = ExceptionMessages.INVALID_STRING
    };

    private static dynamic IsInvalid(int version) => new
    {
        Condition = version < 1,
        Message = ExceptionMessages.Motions.INVALID_VERSION
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPostException = new InvalidMotionException();

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