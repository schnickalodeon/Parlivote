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
    public async Task ShouldRetrieveMotionAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();

        Motion activeMotion = someMotion;
        activeMotion.State = MotionState.Pending;

        Motion expectedMotion = activeMotion.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.GetActiveMotion())
                .ReturnsAsync(activeMotion);

        // Act
        Motion actualMotion =
            await this.pollService.RetrieveActiveAsync();
        
        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.apiBrokerMock.Verify(broker =>
            broker.GetActiveMotion(),
            Times.Once);
        
        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}