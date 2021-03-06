using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    [Fact]
    public async Task ShouldUpdateMotionViewAsyncAsync()
    {
        // Arrange
        dynamic someMotionViewInput = CreateRandomMotionView();

        var someMotionView = new MotionView
        {
            MotionId = someMotionViewInput.Id,
            MeetingId = someMotionViewInput.MeetingId,
            State = ((MotionState)someMotionViewInput.State).GetValue(),
            Text = someMotionViewInput.Text,
            MeetingName = someMotionViewInput.Meeting.Description,
            ApplicantId = someMotionViewInput.ApplicantId,
            ApplicantName = someMotionViewInput.AppicantName,
            VoteViews = new List<VoteView>()
        };

        MotionView inputMotionView = someMotionView;
        MotionView expectedMotionView = inputMotionView;

        var someMotion = new Motion
        {
            Id = someMotionViewInput.Id,
            MeetingId = someMotionViewInput.MeetingId,
            State = someMotionViewInput.State,
            Text = someMotionViewInput.Text,
            Version = someMotionViewInput.Version,
            ApplicantId = someMotionViewInput.ApplicantId,
            Meeting = null,
            Votes = new List<Vote>()
        };

        Motion expectedInputMotion = someMotion;
        Motion returnedMotion = expectedInputMotion;

        Meeting someMeeting = someMotionViewInput.Meeting;
        this.meetingServiceMock.Setup(service =>
            service.RetrieveByIdWithMotionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(someMeeting);

        this.userServiceMock.Setup(service =>
            service.RetrieveByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User() {FirstName = someMotionViewInput.AppicantName});

        this.motionServiceMock.Setup(service =>
            service.ModifyAsync(It.IsAny<Motion>()))
                .ReturnsAsync(returnedMotion);

        // Act
        MotionView actualMotionView =
            await this.motionViewService.UpdateAsync(inputMotionView);

        // Assert
        actualMotionView.Should().BeEquivalentTo(
            expectedMotionView, 
            options => options.Excluding(motionView => motionView.MotionId));

        this.motionServiceMock.Verify(service =>
            service.ModifyAsync(It.Is(SameMotionAs(expectedInputMotion))),
            Times.Once);
    }
}