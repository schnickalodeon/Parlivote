using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Moq;
using Parlivote.Shared.Models.Polls;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Polls;
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly Mock<IApiBroker> apiBrokerMock;
    private readonly IPollService pollService;
    public PollServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.apiBrokerMock = new Mock<IApiBroker>();

        this.pollService = new PollService(
            this.loggingBrokerMock.Object,
            this.apiBrokerMock.Object);
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