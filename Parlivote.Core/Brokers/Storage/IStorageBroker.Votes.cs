using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    Task<Vote> InsertVoteAsync(Vote vote);
    IQueryable<Vote> SelectAllVotes();
    Task<Vote> SelectVoteById(Guid voteId);
    Task<Vote> UpdateVoteAsync(Vote vote);
    Task<Vote> DeleteVoteAsync(Vote vote);
}