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
    public async Task ShouldModifyVoteAsync()
    {
        // Arrange
        Vote someVote = GetRandomVote();
        Vote inputVote = someVote;
        Vote storageVote = inputVote.DeepClone();
        Vote updatedVote = inputVote;
        Vote expectedVote = updatedVote.DeepClone();
        Guid voteId = inputVote.Id;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectVoteById(It.IsAny<Guid>()))
                .ReturnsAsync(storageVote);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdateVoteAsync(It.IsAny<Vote>()))
                .ReturnsAsync(updatedVote);

        // Act
        Vote actualVote =
            await this.voteService.ModifyAsync(inputVote);

        // Assert
        actualVote.Should().BeEquivalentTo(expectedVote);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(voteId),
            Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateVoteAsync(inputVote),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}