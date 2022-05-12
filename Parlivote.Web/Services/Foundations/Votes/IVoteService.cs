using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Web.Services.Foundations.Votes;

public interface IVoteService
{
    Task<Vote> AddAsync(Vote vote);
    Task<List<Vote>> RetrieveAllAsync();
    Task<Vote> RetrieveByIdAsync(Guid motionId);
    Task<Vote> ModifyAsync(Vote vote);
    Task<Vote> DeleteByIdAsync(Guid voteId);
}