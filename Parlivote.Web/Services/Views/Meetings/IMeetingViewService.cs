using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Models.Views.Meetings;

namespace Parlivote.Web.Services.Views.Meetings;

public interface IMeetingViewService
{
    Task<MeetingView> AddAsync(MeetingView meetingView);
    Task<MeetingView> AddAttendance(MeetingView meetingView, Guid userId);
    Task<List<MeetingView>> GetAllAsync();
    Task<List<MeetingView>> GetAllWithMotionsAsync();
    Task<MeetingView> GetByIdWithMotions(Guid meetingId);
    Task<MeetingView> UpdateAsync(MeetingView meetingView);
    Task<Meeting> DeleteByIdAsync(Guid meetingId);
}