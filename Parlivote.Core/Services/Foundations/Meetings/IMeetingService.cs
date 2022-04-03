using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Core.Services.Foundations.Meetings;

public interface IMeetingService
{
    Task<Meeting> AddAsync(Meeting meeting);
    IQueryable<Meeting> RetrieveAll();
    Task<Meeting> RetrieveByIdAsync(Guid meetingId);
    Task<Meeting> ModifyAsync(Meeting meeting);
    Task<Meeting> DeleteMeetingById(Guid meetingId);
}