using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<Vote> PostVoteAsync(Vote vote);
    Task<List<Vote>>GetAllVotesAsync();
    Task<Vote> GetVoteById(Guid voteId);
    Task<Vote> PutVoteAsync(Vote vote);
    Task<Vote> DeleteVoteById(Guid voteId);
    Task<Vote> DeleteVoteByMotionId(Guid motionId);
}