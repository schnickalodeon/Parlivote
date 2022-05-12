using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Core.Services.Foundations.Votes;

public interface IVoteService
{
    Task<Vote> AddAsync(Vote vote);
    IQueryable<Vote> RetrieveAll();
    Task<Vote> RetrieveByIdAsync(Guid voteId);
    Task<Vote> ModifyAsync(Vote vote);
    Task<Vote> RemoveByIdAsync(Guid voteId);
}