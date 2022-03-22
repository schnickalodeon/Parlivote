using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<Poll> PostPollAsync(Poll poll);
    Task<List<Poll>>GetAllPollsAsync();
    Task<Poll> GetPollById(Guid pollId);
    Task<Poll> PutPollAsync(Poll poll);
    Task<Poll> DeletePollById(Guid pollId);
}