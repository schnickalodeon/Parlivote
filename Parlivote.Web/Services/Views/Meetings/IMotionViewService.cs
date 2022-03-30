using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Web.Models.Views.Meetings;

namespace Parlivote.Web.Services.Views.Meetings;

public interface IMeetingViewService
{
    Task<MeetingView> AddAsync(MeetingView meetingView);
    Task<List<MeetingView>> GetAllAsync();
}