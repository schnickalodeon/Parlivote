using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Meetings;
using Parlivote.Shared.Models.Meetings;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    private Mock<ILoggingBroker> loggingBrokerMock;
    private Mock<IStorageBroker> storageBrokerMock;
    private IMeetingService meetingService;

    public MeetingServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.storageBrokerMock = new Mock<IStorageBroker>();

        this.meetingService = new MeetingService(
            this.loggingBrokerMock.Object,
            this.storageBrokerMock.Object);
    }

    private static IQueryable<Meeting> GetRandomMeetings() =>
        GetMeetingFiller(dates: Tests.GetRandomDateTimeOffset())
            .Create(Tests.GetRandomNumber())
            .AsQueryable();

    private static Meeting GetRandomMeeting() =>
        GetMeetingFiller(dates: Tests.GetRandomDateTimeOffset()).Create();

    private static Filler<Meeting> GetMeetingFiller(DateTimeOffset dates)
    {
        var filler = new Filler<Meeting>();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(dates);

        return filler;
    }

}