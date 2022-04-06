using System;
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
    public async Task ShouldModifyMotionAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        Motion inputMotion = someMotion;
        Motion storageMotion = inputMotion.DeepClone();
        Motion updatedMotion = inputMotion;
        Motion expectedMotion = updatedMotion.DeepClone();
        Guid motionId = inputMotion.Id;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMotion);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdateMotionAsync(It.IsAny<Motion>()))
                .ReturnsAsync(updatedMotion);

        // Act
        Motion actualMotion =
            await this.motionService.ModifyAsync(inputMotion);

        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(motionId),
            Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateMotionAsync(inputMotion),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}