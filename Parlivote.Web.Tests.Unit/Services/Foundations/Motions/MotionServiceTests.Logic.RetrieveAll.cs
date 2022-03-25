using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Motions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ShouldRetrieveAllMotionsAsync()
    {
        // Arrange
        List<Motion> randomMotions = GetRandomMotions().ToList();
        List<Motion> apiMotions = randomMotions;
        List<Motion> expectedMotions = apiMotions.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllMotionsAsync())
                .ReturnsAsync(apiMotions);

        // Act
        List<Motion> actualMotions =
            await this.pollService.RetrieveAllAsync();

        // Assert
        actualMotions.Should().BeEquivalentTo(expectedMotions);

        this.apiBrokerMock.Verify(broker => 
            broker.GetAllMotionsAsync(),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}