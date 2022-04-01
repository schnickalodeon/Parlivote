using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Meetings;

public partial class MeetingViewServiceTests
{
    [Fact]
    public async Task ShouldReturnAllMeetingsAsync()
    {
        // given
        List<dynamic> dynamicMeetingViewPropertiesCollection =
            CreateRandomMeetingViewCollections();

        List<Meeting> randomMeetings =
            dynamicMeetingViewPropertiesCollection.Select(property =>
                new Meeting
                {
                    Id = property.Id,
                    Description = property.Description,
                    Start = property.Start,
                    Motions = null
                }).ToList();

        List<Meeting> retrievedMeetings = randomMeetings;

        List<MeetingView> randomMeetingViews =
            dynamicMeetingViewPropertiesCollection.Select(property =>
                new MeetingView
                {
                    Id = property.Id,
                    Description = property.Description,
                    Start = property.Start,
                    Motions = new List<MotionView>()
                }).ToList();

        List<MeetingView> expectedMeetingViews = randomMeetingViews;

        this.meetingServiceMock.Setup(service =>
            service.RetrieveAllAsync())
            .ReturnsAsync(retrievedMeetings);

        // when
        List<MeetingView> retrievedMeetingViews =
            await this.meetingViewService.GetAllAsync();

        // then
        retrievedMeetingViews.Should().BeEquivalentTo(expectedMeetingViews);

        this.meetingServiceMock.Verify(service =>
            service.RetrieveAllAsync(),
            Times.Once());

        this.meetingServiceMock.VerifyNoOtherCalls();
    }
}