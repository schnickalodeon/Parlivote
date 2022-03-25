using System.Collections.Generic;
using System.Linq;
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
    public void ShouldRetrieveAllMotions()
    {
        // Arrange
        IQueryable<Motion> someMotions = GetRandomMotions();
        IQueryable<Motion> storageMotions = someMotions;
        IQueryable<Motion> expectedMotions = storageMotions.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllMotions())
                .Returns(storageMotions);

        // Act
        IQueryable<Motion> actualMotions = this.pollService.RetrieveAll();
        
        // Assert
        actualMotions.Should().BeEquivalentTo(expectedMotions);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllMotions(),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}