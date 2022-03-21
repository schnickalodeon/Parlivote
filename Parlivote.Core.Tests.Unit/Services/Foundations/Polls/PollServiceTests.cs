using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Polls;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public class PollServiceTests
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


}