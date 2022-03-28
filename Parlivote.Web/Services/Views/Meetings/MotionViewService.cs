using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Meetings;
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
        Meeting mappedMeeting = 
            MapToMeeting(meetingView);

        Meeting storageMeeting = 
            await this.meetingService.AddAsync(mappedMeeting);

        MeetingView mappedMeetingView = 
            MapToMeetingView(storageMeeting);

        return mappedMeetingView;
    }

    private static Meeting MapToMeeting(MeetingView meetingView)
    {
        return new Meeting
        {
            Id = meetingView.Id,
            Description = meetingView.Description,
            Start = meetingView.Start
        };
    }

    private static Func<Meeting, MeetingView> AsMeetingView => MapToMeetingView;
    private static MeetingView MapToMeetingView(Meeting meeting)
    {
        return new MeetingView
        {
            Id = meeting.Id,
            Description = meeting.Description,
            Start = meeting.Start
        };
    }
}