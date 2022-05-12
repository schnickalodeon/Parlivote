using System;
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
    public async Task ShouldRetrieveVotesByIdAsync()
    {
        // Arrange
        Vote someVote = Votes.VoteServiceTests.GetRandomVote();
        Vote storageVote = someVote;
        Vote expectedVote = someVote.DeepClone();
        Guid inputVoteId = someVote.Id;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectVoteById(It.IsAny<Guid>()))
                .ReturnsAsync(storageVote);

        // Act
        Vote actualVote = 
            await this.voteService.RetrieveByIdAsync(inputVoteId);
        
        // Assert
        actualVote.Should().BeEquivalentTo(expectedVote);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(inputVoteId),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}