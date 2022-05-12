using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Models.Views.Votes;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Votes;

public partial class VoteViewServiceTests
{
    [Fact]
    public async Task ShouldAddVoteViewAsyncAsync()
    {
        // Arrange
        dynamic someVoteViewInput = CreateRandomVoteView();

        var someVoteView = new VoteView
        {
            VoteId = someVoteViewInput.Id,
            MotionId = someVoteViewInput.MotionId,
            UserId = someVoteViewInput.UserId,
            Value = (VoteValue) someVoteViewInput.Value
        };

        VoteView inputVoteView = someVoteView;
        VoteView expectedVoteView = inputVoteView;

        var someVote = new Vote
        {
            Id = someVoteViewInput.Id,
            MotionId = someVoteViewInput.MotionId,
            UserId = someVoteViewInput.UserId,
            Value = (VoteValue) someVoteViewInput.Value
        };

        Vote expectedInputVote = someVote;
        Vote returnedVote = expectedInputVote;

        this.voteServiceMock.Setup(service =>
            service.AddAsync(It.IsAny<Vote>()))
                .ReturnsAsync(returnedVote);

        // Act
        VoteView actualVoteView =
            await this.voteViewService.AddAsync(inputVoteView);

        // Assert
        actualVoteView.Should().BeEquivalentTo(
            expectedVoteView, 
            options => options.Excluding(voteView => voteView.VoteId));

        this.voteServiceMock.Verify(service =>
            service.AddAsync(It.Is(SameVoteAs(expectedInputVote))),
            Times.Once);
    }
}