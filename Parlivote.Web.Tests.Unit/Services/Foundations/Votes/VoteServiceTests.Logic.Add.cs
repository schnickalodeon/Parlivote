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
    public async Task ShouldAddVoteAsync()
    {
        // Arrange
        Vote someVote = GetRandomVote();
        Vote inputVote = someVote;
        Vote storageVote = inputVote;
        Vote expectedVote = storageVote.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.PostVoteAsync(It.IsAny<Vote>()))
                .ReturnsAsync(storageVote);

        // Act
        Vote actualVote =
            await this.voteService.AddAsync(inputVote);

        // Assert
        actualVote.Should().BeEquivalentTo(expectedVote);

        this.apiBrokerMock.Verify(broker =>
            broker.PostVoteAsync(inputVote),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}