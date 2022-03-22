using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Polls;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Fact]
    public void ShouldRetrieveAllPolls()
    {
        // Arrange
        IQueryable<Poll> somePolls = GetRandomPolls();
        IQueryable<Poll> storagePolls = somePolls;
        IQueryable<Poll> expectedPolls = storagePolls.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllPolls())
                .Returns(storagePolls);

        // Act
        IQueryable<Poll> actualPolls = this.pollService.RetrieveAll();
        
        // Assert
        actualPolls.Should().BeEquivalentTo(expectedPolls);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPolls(),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}