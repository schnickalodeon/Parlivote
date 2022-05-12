using System;
using System.Linq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Parlivote.Shared.Models.VoteValues;

namespace Parlivote.Web.Services.Foundations.Votes;

public partial class VoteService
{
    private void ValidateVote(Vote vote)
    {
        ValidateVoteIsNotNull(vote);

        Validate(
            (IsInvalid(vote.Id), nameof(Vote.Id)),
            (IsInvalid(vote.MotionId), nameof(Vote.MotionId)),
            (IsInvalid(vote.UserId), nameof(Vote.UserId)),
            (IsInvalid(vote.Value), nameof(Vote.Value))
        );
    }

    private void ValidateVoteId(Guid voteId)
    {
        Validate((IsInvalid(voteId), nameof(Vote.Id)));
    }
    private void ValidateStorageVote(Vote maybeVote, Guid voteId)
    {
        if (maybeVote is null)
        {
            throw new NotFoundVoteException(voteId);
        }
    }
    private void ValidateVoteIsNotNull(Vote vote)
    {
        if (vote is null)
        {
            throw new NullVoteException();
        }
    }
    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = ExceptionMessages.INVALID_ID
    };

    private static dynamic IsInvalid(VoteValue value) => new
    {
        Condition = !Enum.GetValues(typeof(VoteValue)).Cast<int>().Contains((int)value),
        Message = ExceptionMessages.Vote.INVALID_VALUE
    };

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPostException = new InvalidVoteException();

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