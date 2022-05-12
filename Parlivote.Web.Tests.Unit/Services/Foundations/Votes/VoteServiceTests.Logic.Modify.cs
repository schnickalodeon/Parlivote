using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Votes;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Votes;

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

        this.apiBrokerMock.Setup(broker =>
            broker.GetVoteById(It.IsAny<Guid>()))
                .ReturnsAsync(storageVote);

        this.apiBrokerMock.Setup(broker =>
            broker.PutVoteAsync(It.IsAny<Vote>()))
                .ReturnsAsync(updatedVote);

        // Act
        Vote actualVote =
            await this.voteService.ModifyAsync(inputVote);

        // Assert
        actualVote.Should().BeEquivalentTo(expectedVote);

        this.apiBrokerMock.Verify(broker =>
            broker.GetVoteById(voteId),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.PutVoteAsync(inputVote),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}