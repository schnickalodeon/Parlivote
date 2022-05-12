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
    public async Task ShouldAddVoteAsync()
    {
        // Arrange
        Vote someVote = GetRandomVote();
        Vote inputVote = someVote;
        Vote storageVote = inputVote;
        Vote expectedVote = storageVote.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()))
                .ReturnsAsync(storageVote);

        // Act
        Vote actualVote =
            await this.voteService.AddAsync(inputVote);

        // Assert
        actualVote.Should().BeEquivalentTo(expectedVote);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(inputVote),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}