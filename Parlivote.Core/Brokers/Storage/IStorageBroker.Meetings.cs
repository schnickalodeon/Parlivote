using Parlivote.Shared.Models.Meetings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    Task<Meeting> InsertMeetingAsync(Meeting meeting);
    IQueryable<Meeting> SelectAllMeetings();
    Task<Meeting> SelectMeetingById(Guid meetingId);
    Task<Meeting> UpdateMeetingAsync(Meeting meeting);
    Task<Meeting> DeleteMeetingAsync(Meeting meeting);
}