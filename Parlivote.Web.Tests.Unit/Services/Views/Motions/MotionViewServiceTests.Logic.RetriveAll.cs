using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Motions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    [Fact]
    public async Task ShouldReturnAllMotionsAsync()
    {
        // given
        List<dynamic> dynamicMotionViewPropertiesCollection =
            CreateRandomMotionViewCollections();

        List<Motion> randomMotions =
            dynamicMotionViewPropertiesCollection.Select(property =>
                new Motion
                {
                    Id = property.Id,
                    AgendaItem = property.AgendaItem,
                    Text = property.Text
                }).ToList();

        List<Motion> retrievedMotions = randomMotions;

        List<MotionView> randomMotionViews =
            dynamicMotionViewPropertiesCollection.Select(property =>
                new MotionView
                {
                    Id = property.Id,
                    AgendaItem = property.AgendaItem,
                    Text = property.Text
                }).ToList();

        List<MotionView> expectedMotionViews = randomMotionViews;

        this.pollServiceMock.Setup(service =>
            service.RetrieveAllAsync())
            .ReturnsAsync(retrievedMotions);

        // when
        List<MotionView> retrievedMotionViews =
            await this.pollViewService.GetAllAsync();

        // then
        retrievedMotionViews.Should().BeEquivalentTo(expectedMotionViews);

        this.pollServiceMock.Verify(service =>
            service.RetrieveAllAsync(),
            Times.Once());

        this.pollServiceMock.VerifyNoOtherCalls();
    }
}