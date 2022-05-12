using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Models.Views.Motions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Votes;

public partial class VoteViewServiceTests 
{
    [Fact]
    public async Task ShouldReturnAllVotesAsync()
    {
        // given
        List<dynamic> dynamicVoteViewPropertiesCollection =
            CreateRandomVoteViewCollections();

        List<Vote> randomVotes =
            dynamicVoteViewPropertiesCollection.Select(property =>
                new Vote
                {
                    Id = property.Id,
                    MotionId = property.MotionId,
                    UserId = property.UserId,
                    Value = (VoteValue)property.Value
                }).ToList();

        List<Vote> retrievedVotes = randomVotes;

        List<VoteView> randomVoteViews =
            dynamicVoteViewPropertiesCollection.Select(property =>
                new VoteView
                {
                    VoteId = property.Id,
                    MotionId = property.MotionId,
                    UserId = property.UserId,
                    Value = (VoteValue)property.Value
                }).ToList();

        List<VoteView> expectedVoteViews = randomVoteViews;

        this.voteServiceMock.Setup(service =>
            service.RetrieveAllAsync())
            .ReturnsAsync(retrievedVotes);

        // when
        List<VoteView> retrievedVoteViews =
            await this.voteViewService.GetAllAsync();

        // then
        retrievedVoteViews.Should().BeEquivalentTo(expectedVoteViews);

        this.voteServiceMock.Verify(service =>
            service.RetrieveAllAsync(),
            Times.Once());

        this.voteServiceMock.VerifyNoOtherCalls();
    }
}