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
    public async Task ShouldAddMotionAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        Motion inputMotion = someMotion;
        Motion storageMotion = inputMotion;
        Motion expectedMotion = storageMotion.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.PostMotionAsync(It.IsAny<Motion>()))
                .ReturnsAsync(storageMotion);

        // Act
        Motion actualMotion =
            await this.pollService.AddAsync(inputMotion);

        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.apiBrokerMock.Verify(broker =>
            broker.PostMotionAsync(inputMotion),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}