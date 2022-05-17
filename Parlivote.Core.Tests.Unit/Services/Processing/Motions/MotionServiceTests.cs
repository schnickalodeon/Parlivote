using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Core.Services.Processing;
using Parlivote.Shared.Models.Motions;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Processing.Motions;

public partial class MotionServiceTests
{
    private Mock<ILoggingBroker> loggingBrokerMock;
    private Mock<IMotionService> motionServiceMock;
    private IMotionProcessingService motionProcessingService;

    public MotionServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.motionServiceMock = new Mock<IMotionService>();

        this.motionProcessingService = new MotionProcessingService(
            this.motionServiceMock.Object,
            this.loggingBrokerMock.Object);
    }

    private static Motion GetRandomMotion() =>
        GetMotionFiller().Create();

    private static IQueryable<Motion> GetRandomMotions(MotionState state)
    {
        return GetMotionFiller(state)
            .Create(count: Tests.GetRandomNumber())
            .AsQueryable();
    }

    private static Filler<Motion> GetMotionFiller(MotionState state = MotionState.Unset)
    {
        var filler = new Filler<Motion>();

        if (state == MotionState.Unset)
        {
            filler.Setup().OnProperty(motion => motion.Meeting).IgnoreIt();
        }
        else
        {
            filler.Setup()
                .OnProperty(motion => motion.State).Use(state)
                .OnProperty(motion => motion.Meeting).IgnoreIt()
                .OnProperty(motion => motion.Votes).IgnoreIt();
        }
        

        return filler;
    }

}