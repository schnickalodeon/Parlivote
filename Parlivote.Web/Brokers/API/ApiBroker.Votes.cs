using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string VotesRelativeUrl = "/api/v1/votes";

    public async Task<Vote> PostVoteAsync(Vote vote) =>
        await this.PostAsync(VotesRelativeUrl, vote);
    public async Task<List<Vote>> GetAllVotesAsync() =>
        await this.GetAsync<List<Vote>>(VotesRelativeUrl);
    public async Task<Vote> GetVoteById(Guid voteId) =>
        await this.GetAsync<Vote>($"{VotesRelativeUrl}/{voteId}");
    public async Task<Vote> PutVoteAsync(Vote vote) =>
        await this.PutAsync(VotesRelativeUrl,vote);
    public async Task<Vote> DeleteVoteById(Guid voteId) =>
        await this.DeleteAsync<Vote>($"{VotesRelativeUrl}/{voteId}");
}