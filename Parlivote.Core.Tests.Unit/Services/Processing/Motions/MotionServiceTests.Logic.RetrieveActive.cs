using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Extensions;
using Parlivote.Shared.Models.Motions;
using MockQueryable.Moq;
using MockQueryable.Core;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Processing.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ShouldRetrieveActiveMotion()
    {
        // Arrange
        List<Motion> someMotions = GetRandomMotions(MotionState.Declined).ToList(); 
        IQueryable<Motion> storageMotions = someMotions.BuildMock();

        Motion activeMotion = someMotions.RandomElement();
        activeMotion.State = MotionState.Pending;

        Motion expectedMotion = activeMotion.DeepClone();

        this.motionServiceMock.Setup(broker =>
            broker.RetrieveAll())
                .Returns(storageMotions);

        // Act
        Motion? actualMotion =
            await this.motionProcessingService.RetrieveActiveAsync();

        // Assert
        actualMotion.Should().BeEquivalentTo(expectedMotion);

        this.motionServiceMock.Verify(broker =>
            broker.RetrieveAll(),
            Times.Once);

        this.motionServiceMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}