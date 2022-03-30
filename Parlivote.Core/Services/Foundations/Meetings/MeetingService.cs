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

    public IQueryable<Meeting> RetrieveAll()
    {
        throw new System.NotImplementedException();
    }
}