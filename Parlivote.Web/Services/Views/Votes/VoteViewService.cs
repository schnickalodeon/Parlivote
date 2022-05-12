using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Foundations.Votes;

namespace Parlivote.Web.Services.Views.Votes;

public class VoteViewService : IVoteViewService
{
    private readonly IVoteService voteService;
    public VoteViewService(IVoteService voteService)
    {
        this.voteService = voteService;
    }
    public async Task<VoteView> AddAsync(VoteView voteView)
    {

        Vote mappedVote = 
            MapToVote(voteView);

        mappedVote.Id = Guid.NewGuid();

        Vote storageVote = 
            await this.voteService.AddAsync(mappedVote);

        VoteView mappedVoteView = 
            MapToVoteView(storageVote);

        return mappedVoteView;
    }

    public async Task<List<VoteView>> GetAllAsync()
    {
        List<Vote> votes =
            await this.voteService.RetrieveAllAsync();

        return votes.Select(AsVoteView).ToList();
    }

    public async Task<VoteView> GetByIdAsync(Guid voteId)
    {
        Vote vote =
            await this.voteService.RetrieveByIdAsync(voteId);

        return MapToVoteView(vote);
    }

    public async Task<VoteView> UpdateAsync(VoteView voteView)
    {
        Vote voteToUpdate = MapToVote(voteView);
        
        Vote updatedVote =
            await this.voteService.ModifyAsync(voteToUpdate);

        VoteView updatedVoteView = MapToVoteView(updatedVote);

        return updatedVoteView;
    }

    public async Task<Vote> DeleteByIdAsync(Guid voteId)
    {
        return await this.voteService.DeleteByIdAsync(voteId);
    }

    private static Func<Vote, VoteView> AsVoteView => MapToVoteView;

    private static Vote MapToVote(VoteView voteView)
    {
        return new Vote
        {
            Id = voteView.VoteId,
            MotionId = voteView.MotionId,
            UserId = voteView.UserId,
            Value = voteView.Value
        };
    }
    private static VoteView MapToVoteView(Vote vote)
    {
        return new VoteView
        {
            VoteId = vote.Id,
            MotionId = vote.MotionId,
            UserId = vote.UserId,
            Value = vote.Value
        };
    }
}