using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Polls;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Fact]
    public async Task ShouldAddPollAsync()
    {
        // Arrange
        Poll somePoll = GetRandomPoll();
        Poll inputPoll = somePoll;
        Poll storagePoll = inputPoll;
        Poll expectedPoll = storagePoll.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()))
                .ReturnsAsync(storagePoll);

        // Act
        Poll actualPoll =
            await this.pollService.AddPollAsync(inputPoll);

        // Assert
        actualPoll.Should().BeEquivalentTo(expectedPoll);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(inputPoll),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}