using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;

namespace Parlivote.Web.Services.Foundations.Meetings;

public partial class MeetingService : IMeetingService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IApiBroker apiBroker;

    public MeetingService(ILoggingBroker loggingBroker, IApiBroker apiBroker)
    {
        this.loggingBroker = loggingBroker;
        this.apiBroker = apiBroker;
    }

    public Task<Meeting> AddAsync(Meeting meeting) =>
        TryCatch(async () => 
        {
            ValidateMeeting(meeting);
            return await this.apiBroker.PostMeetingAsync(meeting);
        });

    public Task<List<Meeting>> RetrieveAllAsync() =>
        TryCatch(async () =>
        {
            return await this.apiBroker.GetAllMeetingsAsync();
        });

    public Task<Meeting> RetrieveByIdAsync(Guid meetingId) =>
        TryCatch(async () =>
        {
            ValidateMeetingId(meetingId);
            return await this.apiBroker.GetMeetingById(meetingId);
        });

    public Task<List<Meeting>> RetrieveAllWithMotionsAsync() =>
        TryCatch(async () =>
        {
            return await this.apiBroker.GetAllMeetingsWithMotionsAsync();
        });

    public Task<Meeting> RetrieveByIdWithMotionsAsync(Guid meetingId) =>
        TryCatch(async () =>
        {
            ValidateMeetingId(meetingId);
            return await this.apiBroker.GetMeetingByIdWithMotionsAsync(meetingId);
        });

    public Task<Meeting> ModifyAsync(Meeting meeting) =>
        TryCatch(async () =>
        {
            ValidateMeeting(meeting);

            Meeting maybeMeeting =
                await this.apiBroker.GetMeetingById(meeting.Id);

            ValidateStorageMeeting(maybeMeeting, meeting.Id);

            return await this.apiBroker.PutMeetingAsync(meeting);
        });

    public async Task<Meeting> DeleteByIdAsync(Guid meetingId)
    {
        return await this.apiBroker.DeleteMeetingById(meetingId);
    }
}