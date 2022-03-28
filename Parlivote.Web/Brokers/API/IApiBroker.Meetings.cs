using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<Meeting> PostMeetingAsync(Meeting meeting);
    Task<List<Meeting>>GetAllMeetingsAsync();
    Task<Meeting> GetMeetingById(Guid meetingId);
    Task<Meeting> PutMeetingAsync(Meeting meeting);
    Task<Meeting> DeleteMeetingById(Guid meetingId);
}