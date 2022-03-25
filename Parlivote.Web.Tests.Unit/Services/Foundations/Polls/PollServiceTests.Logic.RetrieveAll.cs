using System.Collections.Generic;
using System.Linq;
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
    public async Task ShouldRetrieveAllPollsAsync()
    {
        // Arrange
        List<Poll> randomPolls = GetRandomPolls().ToList();
        List<Poll> apiPolls = randomPolls;
        List<Poll> expectedPolls = apiPolls.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllPollsAsync())
                .ReturnsAsync(apiPolls);

        // Act
        List<Poll> actualPolls =
            await this.pollService.RetrieveAllAsync();

        // Assert
        actualPolls.Should().BeEquivalentTo(expectedPolls);

        this.apiBrokerMock.Verify(broker => 
            broker.GetAllPollsAsync(),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}