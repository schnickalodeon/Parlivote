using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;

namespace Parlivote.Web.Services.Foundations.Votes;

public partial class VoteService : IVoteService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IApiBroker apiBroker;

    public VoteService(ILoggingBroker loggingBroker, IApiBroker apiBroker)
    {
        this.loggingBroker = loggingBroker;
        this.apiBroker = apiBroker;
    }

    public Task<Vote> AddAsync(Vote vote) =>
        TryCatch(async () => 
        {
            ValidateVote(vote);
            return await this.apiBroker.PostVoteAsync(vote);
        });

    public Task<List<Vote>> RetrieveAllAsync() =>
        TryCatch(async () =>
        {
            return await this.apiBroker.GetAllVotesAsync();
        });

    public Task<Vote> RetrieveByIdAsync(Guid voteId) =>
        TryCatch(async () =>
        {
            ValidateVoteId(voteId);
            return await this.apiBroker.GetVoteById(voteId);
        });

    public Task<Vote> ModifyAsync(Vote vote) =>
        TryCatch(async () =>
        {
            ValidateVote(vote);

            Vote maybeVote =
                await this.apiBroker.GetVoteById(vote.Id);

            ValidateStorageVote(maybeVote, vote.Id);

            return await this.apiBroker.PutVoteAsync(vote);
        });

    public async Task<Vote> DeleteByIdAsync(Guid voteId)
    {
        return await this.apiBroker.DeleteVoteById(voteId);
    }
}