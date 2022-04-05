using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<Meeting> PostMeetingAsync(Meeting meeting);
    Task<List<Meeting>>GetAllMeetingsAsync();
    Task<List<Meeting>> GetAllMeetingsWithMotionsAsync();
    Task<Meeting> GetMeetingByIdWithMotionsAsync(Guid meetingId);
    Task<Meeting> GetMeetingById(Guid meetingId);
    Task<Meeting> PutMeetingAsync(Meeting meeting);
    Task<Meeting> DeleteMeetingById(Guid meetingId);
}