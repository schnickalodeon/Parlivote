using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Polls;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls;

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

        this.apiBrokerMock.Setup(broker =>
            broker.PostPollAsync(It.IsAny<Poll>()))
                .ReturnsAsync(storagePoll);

        // Act
        Poll actualPoll =
            await this.pollService.AddAsync(inputPoll);

        // Assert
        actualPoll.Should().BeEquivalentTo(expectedPoll);

        this.apiBrokerMock.Verify(broker =>
            broker.PostPollAsync(inputPoll),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}