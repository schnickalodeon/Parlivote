using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            MapToMeetingView(storageMeeting);

        return mappedMeetingView;
    }

    public async Task<MeetingView> AddAttendance(MeetingView meetingView, Guid userId)
    {
        User user = await this.userService.RetrieveByIdAsync(userId);

        meetingView.Attendances.Add(user);

        MeetingView view = await UpdateAsync(meetingView);

        return view;
    }

    public async Task<MeetingView> RemoveAttendance(MeetingView meetingView, Guid userId)
    {
        var removed = meetingView.Attendances.RemoveAll(user => user.Id == userId);

        MeetingView view = await UpdateAsync(meetingView);

        return view;
    }

    public async Task<List<MeetingView>> GetAllAsync()
    {
        List<Meeting> meetings =
            await this.meetingService.RetrieveAllAsync();

        return meetings.Select(AsMeetingView).ToList();
    }

    public async Task<List<MeetingView>> GetAllWithMotionsAsync()
    {
        List<Meeting> meetings =
            await this.meetingService.RetrieveAllWithMotionsAsync();

        return meetings.Select(AsMeetingView).ToList();
    }

    public async Task<MeetingView> GetByIdWithMotions(Guid meetingId)
    {
        Meeting meeting =
            await this.meetingService.RetrieveByIdWithMotionsAsync(meetingId);

        return MapToMeetingView(meeting);
    }

    public async Task<MeetingView> UpdateAsync(MeetingView meetingView)
    {
        Meeting meetingToUpdate = MapToMeeting(meetingView);
        
        Meeting updatedMeeting =
            await this.meetingService.ModifyAsync(meetingToUpdate);

        MeetingView updatedMeetingView = MapToMeetingView(updatedMeeting);

        return updatedMeetingView;
    }

    public async Task<Meeting> DeleteByIdAsync(Guid meetingId)
    {
        return await this.meetingService.DeleteByIdAsync(meetingId);
    }

    private static Func<Meeting, MeetingView> AsMeetingView => MapToMeetingView;
    private static Func<Motion, MotionView> AsMotionView => MapToMotionView;
    private static Func<MotionView, Motion> AsMotion => MapToMotion;

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
    private static MeetingView MapToMeetingView(Meeting meeting)
    {
        return new MeetingView
        {
            Id = meeting.Id,
            Description = meeting.Description,
            Start = meeting.Start,
            Motions = meeting.Motions?.Select(AsMotionView).ToList() ?? new List<MotionView>(),
        };
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
    private static MotionView MapToMotionView(Motion motion)
    {
        return new MotionView
        {
            MotionId = motion.Id,
            Version = motion.Version,
            State = motion.State.GetValue(),
            MeetingId = motion.MeetingId,
            Text = motion.Text,
            VoteViews = motion.Votes?.Select(AsVoteView).ToList() ?? new List<VoteView>()
        };
    }
    private static Motion MapToMotion(MotionView motionView)
    {
        return new Motion
        {
            Id = motionView.MotionId,
            Version = motionView.Version,
            State = MotionStateConverter.FromString(motionView.State),
            MeetingId = motionView.MeetingId,
            Text = motionView.Text,
            Votes = motionView.VoteViews?.Select(AsVote).ToList() ?? new List<Vote>()
        };
    }
}