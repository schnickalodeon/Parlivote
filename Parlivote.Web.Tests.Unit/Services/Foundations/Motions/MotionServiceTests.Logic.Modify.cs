using System;
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
    public async Task ShouldModifyMotionAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        Motion inputMotion = someMotion;
        Motion storageMotion = inputMotion.DeepClone();
        Motion updatedMotion = inputMotion;
        Motion expectedMotion = updatedMotion.DeepClone();
        Guid motionId = inputMotion.Id;

        this.apiBrokerMock.Setup(broker =>
            broker.GetMotionById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMotion);

        this.apiBrokerMock.Setup(broker =>
            broker.PutMotionAsync(It.IsAny<Motion>()))
                .ReturnsAsync(updatedMotion);

        // Act
        Motion actualMotion =
            await this.motionService.ModifyAsync(inputMotion);

        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMotionById(motionId),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.PutMotionAsync(inputMotion),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}