using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Votes;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public void ShouldRetrieveAllVotes()
    {
        // Arrange
        IQueryable<Vote> someVotes = GetRandomVotes();
        IQueryable<Vote> storageVotes = someVotes;
        IQueryable<Vote> expectedVotes = storageVotes.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllVotes())
                .Returns(storageVotes);

        // Act
        IQueryable<Vote> actualVotes = this.voteService.RetrieveAll();
        
        // Assert
        actualVotes.Should().BeEquivalentTo(expectedVotes);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllVotes(),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}