using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Core.Services.Foundations.Meetings;

public partial class MeetingService : IMeetingService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IStorageBroker storageBroker;

    public MeetingService(ILoggingBroker loggingBroker, IStorageBroker storageBroker)
    {
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
    }

    public Task<Meeting> AddAsync(Meeting meeting) =>
        TryCatch(async () =>
        {
            ValidateMeeting(meeting);
            return await this.storageBroker.InsertMeetingAsync(meeting);
        });

    public IQueryable<Meeting> RetrieveAll() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllMeetings();
        });

    public Task<Meeting> RetrieveByIdAsync(Guid meetingId) =>
        TryCatch(async () =>
        {
            ValidateMeetingId(meetingId);
            return await this.storageBroker.SelectMeetingById(meetingId);
        });

    public Task<Meeting> ModifyAsync(Meeting meeting) =>
        TryCatch(async () =>
        {
            ValidateMeeting(meeting);
            Meeting maybeMeeting =
                await this.storageBroker.SelectMeetingById(meeting.Id);

            ValidateStorageMeeting(maybeMeeting, meeting.Id);

            var updatedMeeting = await this.storageBroker.UpdateMeetingAsync(meeting);

            return updatedMeeting;
        });

    public async Task<Meeting> DeleteMeetingById(Guid meetingId)
    {
        Meeting meetingToDelete =
            await this.storageBroker.SelectMeetingById(meetingId);

        return await this.storageBroker.DeleteMeetingAsync(meetingToDelete);
    }
}