using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Polls;
using Parlivote.Web.Models.Views.Polls;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Polls;

public partial class PollViewServiceTests
{
    [Fact]
    public async Task ShouldReturnAllPollsAsync()
    {
        // given
        List<dynamic> dynamicPollViewPropertiesCollection =
            CreateRandomPollViewCollections();

        List<Poll> randomPolls =
            dynamicPollViewPropertiesCollection.Select(property =>
                new Poll
                {
                    Id = property.Id,
                    AgendaItem = property.AgendaItem,
                    Text = property.Text
                }).ToList();

        List<Poll> retrievedPolls = randomPolls;

        List<PollView> randomPollViews =
            dynamicPollViewPropertiesCollection.Select(property =>
                new PollView
                {
                    Id = property.Id,
                    AgendaItem = property.AgendaItem,
                    Text = property.Text
                }).ToList();

        List<PollView> expectedPollViews = randomPollViews;

        this.pollServiceMock.Setup(service =>
            service.RetrieveAllAsync())
            .ReturnsAsync(retrievedPolls);

        // when
        List<PollView> retrievedPollViews =
            await this.pollViewService.GetAllAsync();

        // then
        retrievedPollViews.Should().BeEquivalentTo(expectedPollViews);

        this.pollServiceMock.Verify(service =>
            service.RetrieveAllAsync(),
            Times.Once());

        this.pollServiceMock.VerifyNoOtherCalls();
    }
}