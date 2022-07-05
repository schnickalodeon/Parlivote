using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using NuGet.Packaging;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Foundations.Users;
using Parlivote.Web.Services.Foundations.Votes;

namespace Parlivote.Web.Services.Views.Motions;

public class MotionViewService : IMotionViewService
{
    private readonly IMotionService motionService;
    private readonly IVoteService voteService;
    private readonly IMeetingService meetingService;
    private readonly IUserService userService;
    public MotionViewService(
        IMotionService motionService, 
        IMeetingService meetingService, 
        IUserService userService, 
        IVoteService voteService)
    {
        this.motionService = motionService;
        this.meetingService = meetingService;
        this.userService = userService;
        this.voteService = voteService;
    }
    public async Task<MotionView> AddAsync(MotionView motionView)
    {
        var mappedMotion = new Motion
        {
            Id = Guid.NewGuid(),
            MeetingId = motionView.MeetingId,
            State = MotionState.Submitted,
            Text = motionView.Text,
            Title = motionView.Title,
            ApplicantId = motionView.ApplicantId
        };

            Motion storageMotion = 
            await this.motionService.AddAsync(mappedMotion);

        MotionView mappedMotionView = 
            await MapToMotionView(storageMotion);

        return mappedMotionView;
    }

    public async Task<List<MotionView>> GetAllAsync()
    {
        List<Motion> motions = 
            await this.motionService.RetrieveAllAsync();

        return await MapToMotionViews(motions);
    }

    public async Task<List<MotionView>> GetAllWithMeetingAsync()
    {
        List<Motion> motions =
            await this.motionService.RetrieveAllAsync();

        List<MotionView> motionViews =
            await MapToMotionViews(motions);

        return motionViews;
    }

    public async Task<List<MotionView>> GetMyWithMeetingAsync(Guid applicantId)
    {
        List<Motion> motions =
            await this.motionService.RetrieveByApplicantId(applicantId);

        List<MotionView> motionViews =
            await MapToMotionViews(motions);

        return motionViews;
    }

    public async Task<MotionView> GetActiveAsync()
    {
        Motion activeMotion =
            await this.motionService.RetrieveActiveAsync();

        if (activeMotion is null)
        {
            return null;
        }

        return await MapToMotionView(activeMotion);
    }

    public async Task<MotionView> UpdateAsync(MotionView motionView)
    {
        Motion mappedMotion = MapToMotion(motionView);

        Motion updatedMotion =
            await this.motionService.ModifyAsync(mappedMotion);

        MotionView updatedMotionView =
            await MapToMotionView(updatedMotion);

        if (motionView.State == MotionStateConverter.Pending)
        {
            List<Vote> votes = await AddEmptyVoteEntries(updatedMotionView);
            List<VoteView> voteViews = votes.Select(AsVoteView).ToList();
            updatedMotionView.VoteViews.AddRange(voteViews);
        }

        return updatedMotionView;
    }

    private async Task<List<Vote>> AddEmptyVoteEntries(MotionView motionView)
    {
        List<User> attendantUsers =
            await this.userService.RetrieveAttendantAsync();

        List<Vote> votes = attendantUsers.Select(user => new Vote()
        {
            Id = Guid.NewGuid(),
            MotionId = motionView.MotionId,
            UserId = user.Id,
            Value = VoteValue.NoValue
        }).ToList();

        await this.voteService.DeleteByMotionIdAsync(motionView.MotionId);
        return await this.voteService.AddRangeAsync(votes);
    }

    public async Task<MotionView> RemoveByIdAsync(Guid motionIdToDelete)
    {
        Motion deletedMotion =
            await this.motionService.RemoveByIdAsync(motionIdToDelete);

        return await MapToMotionView(deletedMotion);
    }

    private Motion MapToMotion(MotionView motionView)
    {
        return new Motion
        {
            Id = motionView.MotionId,
            MeetingId = motionView.MeetingId,
            ApplicantId = motionView.ApplicantId,
            State = MotionStateConverter.FromString(motionView.State),
            Text = motionView.Text,
            Title = motionView.Title,
            Votes = motionView.VoteViews?.Select(AsVote).ToList() ?? new List<Vote>()
        };
    }
    private async Task<MotionView> MapToMotionView(Motion motion)
    {
        string meetingName = await GetMeetingDescription(motion.MeetingId);
        string applicantName = await GetApplicantName(motion.ApplicantId);

        return new MotionView
        {
            MotionId = motion.Id,
            MeetingId = motion.MeetingId,
            ApplicantId = motion.ApplicantId,
            ApplicantName = applicantName,
            State = motion.State.GetValue(),
            Title = motion.Title,
            Text = motion.Text,
            MeetingName = meetingName,
            VoteViews = motion.Votes?.Select(AsVoteView).ToList()
        };
    }

    private async Task<string> GetApplicantName(Guid? userId)
    {
        if (!userId.HasValue)
        {
            return string.Empty;
        }

        User applicant =
            await this.userService.RetrieveByIdAsync(userId.Value);

        return applicant?.FirstName ?? string.Empty;
    }

    private async Task<string> GetMeetingDescription(Guid? meetingId)
    {
        if (!meetingId.HasValue)
        {
            return string.Empty;
        }

        Meeting meeting =
            await this.meetingService.RetrieveByIdWithMotionsAsync(meetingId.Value);

        return meeting?.Description ?? string.Empty;
    }

    

    private Func<Motion, Task<MotionView>> AsMotionView => MapToMotionView;
    private Func<Vote, VoteView> AsVoteView => MapToVoteView;

    private Func<VoteView, Vote> AsVote => MapToVote;

    private Vote MapToVote(VoteView voteView)
    {
        var vote = new Vote
        {
            Id = voteView.VoteId,
            UserId = voteView.UserId,
            MotionId = voteView.MotionId,
            Value = voteView.Value,
        };

        return vote;
    }
    private VoteView MapToVoteView(Vote vote)
    {
        var voteView = new VoteView
        {
            VoteId = vote.Id,
            UserId = vote.UserId,
            MotionId = vote.MotionId,
            Value = vote.Value,
        };

        return voteView;
    }
   
    private async Task<List<MotionView>> MapToMotionViews(List<Motion> motions)
    {
        var motionViews = new List<MotionView>();
        foreach (Motion motion in motions)
        {
            MotionView motionView = await MapToMotionView(motion);
            motionViews.Add(motionView);
        }

        return motionViews;
    }
    
}