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
                    Version = property.Version,
                    State = property.State,
                    Text = property.Text,
                    Meeting = null,
                    MeetingId = null,
                }).ToList();

        List<Motion> retrievedMotions = randomMotions;

        List<MotionView> randomMotionViews =
            dynamicMotionViewPropertiesCollection.Select(property =>
                new MotionView
                {
                    MotionId = property.Id,
                    Version = property.Version,
                    State = ((MotionState)property.State).GetValue(),
                    Text = property.Text,
                    MeetingName = "",
                }).ToList();

        List<MotionView> expectedMotionViews = randomMotionViews;

        this.motionServiceMock.Setup(service =>
            service.RetrieveAllAsync())
            .ReturnsAsync(retrievedMotions);

        // when
        List<MotionView> retrievedMotionViews =
            await this.motionViewService.GetAllAsync();

        // then
        retrievedMotionViews.Should().BeEquivalentTo(expectedMotionViews);

        this.motionServiceMock.Verify(service =>
            service.RetrieveAllAsync(),
            Times.Once());

        this.motionServiceMock.VerifyNoOtherCalls();
    }
}