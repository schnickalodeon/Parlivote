using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Core.Services.Foundations.Votes;

public partial class VoteService : IVoteService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IStorageBroker storageBroker;

    public VoteService(ILoggingBroker loggingBroker, IStorageBroker storageBroker)
    {
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
    }

    public Task<Vote> AddAsync(Vote vote) =>
        TryCatch(async () =>
        {
            ValidateVote(vote);
            ValidateUserHasNotVoted(vote);
            return await this.storageBroker.InsertVoteAsync(vote);
        });

    public IQueryable<Vote> RetrieveAll() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllVotes();
        });

    public Task<Vote> RetrieveByIdAsync(Guid voteId) =>
        TryCatch(async () =>
        {
            ValidateVoteId(voteId);
            return await this.storageBroker.SelectVoteById(voteId);
        });

    public Task<Vote> ModifyAsync(Vote vote) =>
        TryCatch(async () =>
        {
            ValidateVote(vote);
            Vote maybeVote =
                await this.storageBroker.SelectVoteById(vote.Id);

            ValidateStorageVote(maybeVote, vote.Id);

            return await this.storageBroker.UpdateVoteAsync(vote);
        });

    public Task<Vote> RemoveByIdAsync(Guid voteId) =>
        TryCatch(async () =>
        {
            ValidateVoteId(voteId);

            Vote voteToDelete =
                await this.storageBroker.SelectVoteById(voteId);

            return await this.storageBroker.DeleteVoteAsync(voteToDelete);
        });

    public Task<Vote> RemoveByIdMotionIdAsync(Guid motionId) =>
        TryCatch(async () =>
        {
            ValidateVoteId(motionId);

            IEnumerable<Vote> votesToDelete =
                await this.storageBroker.SelectAllVotes().Where(vote => vote.MotionId == motionId).ToListAsync();

            foreach (Vote vote in votesToDelete)
            {
                await this.storageBroker.DeleteVoteAsync(vote);
            }

            return votesToDelete.FirstOrDefault();
        });
}