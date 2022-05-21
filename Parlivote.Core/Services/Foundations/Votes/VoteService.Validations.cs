using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Parlivote.Shared.Models.VoteValues;

namespace Parlivote.Core.Services.Foundations.Votes;

public partial class VoteService
{
    private static void ValidateVote(Vote vote)
    {
        ValidateVoteIsNotNull(vote);
        
        Validate(
            (IsInvalid(vote.Id), nameof(Vote.Id)),
            (IsInvalid(vote.MotionId), nameof(Vote.MotionId)),
            (IsInvalid(vote.UserId), nameof(Vote.UserId)),
            (IsInvalid(vote.Value), nameof(Vote.Value))
        );
    }

    private static void ValidateVoteIsNotNull(Vote vote)
    {
        if (vote is null)
        {
            throw new NullVoteException();
        }
    }

    private static void ValidateVoteId(Guid voteId)
    {
        Validate((IsInvalid(voteId), nameof(Vote.Id)));
    }
    private static void ValidateStorageVote(Vote maybeVote, Guid voteId)
    {
        if (maybeVote is null)
        {
            throw new NotFoundVoteException(voteId);
        }
    }

    private void ValidateUserHasNotVoted(Vote maybeVote)
    {
        IQueryable<Vote> votes = RetrieveAll();

        bool Predicate(Vote vote) =>
            vote.MotionId == maybeVote.MotionId && vote.UserId == maybeVote.UserId;

        bool userHasVoted = votes.Any(Predicate);

        if (userHasVoted)
        {
            throw new AlreadyVotedException();
        }
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = ExceptionMessages.INVALID_ID
    };

    private static dynamic IsInvalid(VoteValue value) => new
    {
        Condition = !Enum.GetValues(typeof(VoteValue)).Cast<int>().Contains((int) value),
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