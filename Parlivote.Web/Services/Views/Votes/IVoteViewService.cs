using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Models.Views.Votes;

namespace Parlivote.Web.Services.Views.Votes;

public interface IVoteViewService
{
    Task<VoteView> AddAsync(VoteView voteView);
    Task<List<VoteView>> GetAllAsync();
    Task<VoteView> GetByIdAsync(Guid voteId);
    Task<VoteView> UpdateAsync(VoteView voteView);
    Task<Vote> DeleteByIdAsync(Guid voteId);
}