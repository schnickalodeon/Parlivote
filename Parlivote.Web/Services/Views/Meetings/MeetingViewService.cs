﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Foundations.Meetings;

namespace Parlivote.Web.Services.Views.Meetings;

public class MeetingViewService : IMeetingViewService
{
    private readonly IMeetingService meetingService;
    public MeetingViewService(IMeetingService meetingService)
    {
        this.meetingService = meetingService;
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

    private static Meeting MapToMeeting(MeetingView meetingView)
    {
        return new Meeting
        {
            Id = meetingView.Id.Value,
            Description = meetingView.Description,
            Start = meetingView.Start
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
}