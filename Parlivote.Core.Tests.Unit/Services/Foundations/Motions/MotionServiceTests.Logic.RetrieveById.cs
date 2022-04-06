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
    public async Task ShouldRetrieveMotionsByIdAsync()
    {
        // Arrange
        Motion someMotion = Motions.MotionServiceTests.GetRandomMotion();
        Motion storageMotion = someMotion;
        Motion expectedMotion = someMotion.DeepClone();
        Guid inputMotionId = someMotion.Id;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMotion);

        // Act
        Motion actualMotion = 
            await this.motionService.RetrieveByIdAsync(inputMotionId);
        
        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(inputMotionId),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}