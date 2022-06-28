using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Extensions;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Users;

namespace Parlivote.Web.Services.Views.Meetings;

public class MeetingViewService : IMeetingViewService
{
    private readonly IMeetingService meetingService;
    private readonly IUserService userService;
    public MeetingViewService(IMeetingService meetingService, IUserService userService)
    {
        this.meetingService = meetingService;
        this.userService = userService;
    }
    public async Task<MeetingView> AddAsync(MeetingView meetingView)
    {
        meetingView.Id = Guid.NewGuid();

        Meeting mappedMeeting = 
            MapToMeeting(meetingView);

        Meeting storageMeeting = 
            await this.meetingService.AddAsync(mappedMeeting);

        MeetingView mappedMeetingView = 
            await MapToMeetingView(storageMeeting);

        return mappedMeetingView;
    }

    public async Task<List<MeetingView>> GetAllAsync()
    {
        List<Meeting> meetings =
            await this.meetingService.RetrieveAllAsync();

        return await MapToMeetingViews(meetings);
    }

    public async Task<List<MeetingView>> GetAllWithMotionsAsync()
    {
        List<Meeting> meetings =
            await this.meetingService.RetrieveAllWithMotionsAsync();

        return await MapToMeetingViews(meetings);
    }

    public async Task<MeetingView> GetByIdWithMotions(Guid meetingId)
    {
        Meeting meeting =
            await this.meetingService.RetrieveByIdWithMotionsAsync(meetingId);

        return await MapToMeetingView(meeting);
    }

    public async Task<MeetingView> UpdateAsync(MeetingView meetingView)
    {
        Meeting meetingToUpdate = MapToMeeting(meetingView);
        
        Meeting updatedMeeting =
            await this.meetingService.ModifyAsync(meetingToUpdate);

        MeetingView updatedMeetingView = 
            await MapToMeetingView(updatedMeeting);

        return updatedMeetingView;
    }

    public async Task<Meeting> DeleteByIdAsync(Guid meetingId)
    {
        return await this.meetingService.DeleteByIdAsync(meetingId);
    }

    private Func<Meeting, Task<MeetingView>> AsMeetingView => MapToMeetingView;

    private static Meeting MapToMeeting(MeetingView meetingView)
    {
        return new Meeting
        {
            Id = meetingView.Id.Value,
            Description = meetingView.Description,
            Start = meetingView.Start,
            Motions = meetingView.Motions?.Select(AsMotion).ToList() ?? new List<Motion>(),
        };
    }
    private async Task<MeetingView> MapToMeetingView(Meeting meeting)
    {
        List<MotionView> motionViews = (meeting.Motions is null)
            ? new List<MotionView>()
            : await MapToMotionViews(meeting.Motions);
        
        return new MeetingView
        {
            Id = meeting.Id,
            Description = meeting.Description,
            Start = meeting.Start,
            Motions = motionViews,
        };
    }

    private async Task<List<MeetingView>> MapToMeetingViews(List<Meeting> meetings)
    {
        List<MeetingView> meetingViews = new List<MeetingView>();
        foreach (Meeting meeting in meetings)
        {
            MeetingView mappedMeetingView = await MapToMeetingView(meeting);
            meetingViews.Add(mappedMeetingView);
        }

        return meetingViews;
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

    private static Func<Vote, VoteView> AsVoteView => MapToVoteView;
    private static Func<VoteView, Vote> AsVote => MapToVote;
    private static VoteView MapToVoteView(Vote vote)
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
    private static Vote MapToVote(VoteView voteView)
    {
        var vote = new Vote()
        {
            Id = voteView.VoteId,
            UserId = voteView.UserId,
            MotionId = voteView.MotionId,
            Value = voteView.Value,
        };

        return vote;
    }

    private Func<Motion, Task<MotionView>> AsMotionView => MapToMotionView;
    private static Func<MotionView, Motion> AsMotion => MapToMotion;
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
    private static Motion MapToMotion(MotionView motionView)
    {
        return new Motion
        {
            Id = motionView.MotionId,
            State = MotionStateConverter.FromString(motionView.State),
            MeetingId = motionView.MeetingId,
            ApplicantId = motionView.ApplicantId,
            Title = motionView.Title,
            Text = motionView.Text,
            Votes = motionView.VoteViews?.Select(AsVote).ToList() ?? new List<Vote>()
        };
    }
}