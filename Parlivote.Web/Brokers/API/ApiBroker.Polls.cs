using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string PollsRelativeUrl = "/api/v1/polls";

    public async Task<Poll> PostPollAsync(Poll poll) =>
        await this.PostAsync(PollsRelativeUrl, poll);

    public async Task<List<Poll>> GetAllPollsAsync() =>
        await this.GetAsync<List<Poll>>(PollsRelativeUrl);
    

    public async Task<Poll> GetPollById(Guid pollId) =>
        await this.GetAsync<Poll>($"{PollsRelativeUrl}/{pollId}");

    public async Task<Poll> PutPollAsync(Poll poll) =>
        await this.PutAsync(PollsRelativeUrl,poll);

    public async Task<Poll> DeletePollById(Guid pollId) =>
        await this.DeleteAsync<Poll>($"{PollsRelativeUrl}/{pollId}");
}