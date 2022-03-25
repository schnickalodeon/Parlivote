using System.Collections.Generic;
using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Shared.Models.Motions;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    private Mock<ILoggingBroker> loggingBrokerMock;
    private Mock<IStorageBroker> storageBrokerMock;
    private IMotionService pollService;

    public MotionServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.storageBrokerMock = new Mock<IStorageBroker>();

        this.pollService = new MotionService(
            this.loggingBrokerMock.Object,
            this.storageBrokerMock.Object);
    }

    private static Motion GetRandomMotion() =>
        GetMotionFiller().Create();

    private static IQueryable<Motion> GetRandomMotions()
    {
        return GetMotionFiller()
            .Create(count: Tests.GetRandomNumber())
            .AsQueryable();
    }

    private static Filler<Motion> GetMotionFiller()
    {
        return new Filler<Motion>();
    }

}