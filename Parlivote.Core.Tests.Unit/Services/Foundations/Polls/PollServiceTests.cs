using System.Collections.Generic;
using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Polls;
using Parlivote.Shared.Models.Polls;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    private Mock<ILoggingBroker> loggingBrokerMock;
    private Mock<IStorageBroker> storageBrokerMock;
    private IPollService pollService;

    public PollServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.storageBrokerMock = new Mock<IStorageBroker>();

        this.pollService = new PollService(
            this.loggingBrokerMock.Object,
            this.storageBrokerMock.Object);
    }

    private static Poll GetRandomPoll() =>
        GetPollFiller().Create();

    private static IQueryable<Poll> GetRandomPolls()
    {
        return GetPollFiller()
            .Create(count: Tests.GetRandomNumber())
            .AsQueryable();
    }

    private static Filler<Poll> GetPollFiller()
    {
        return new Filler<Poll>();
    }

}