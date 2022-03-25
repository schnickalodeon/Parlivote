using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Motions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

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

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()))
                .ReturnsAsync(storageMotion);

        // Act
        Motion actualMotion =
            await this.pollService.AddAsync(inputMotion);

        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(inputMotion),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}