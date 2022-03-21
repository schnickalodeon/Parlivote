using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    Task<Poll> InsertPollAsync(Poll poll);
    IQueryable<Poll> SelectAllPolls();
    Task<Poll> SelectPollById(Guid pollId);
    Task<Poll> UpdatePollAsync(Poll poll);
    Task<Poll> DeletePollAsync(Poll poll);
}