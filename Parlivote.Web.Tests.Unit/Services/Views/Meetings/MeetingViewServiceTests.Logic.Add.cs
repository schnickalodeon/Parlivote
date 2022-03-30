﻿using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Models.Views.Meetings;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Meetings;

public partial class MeetingViewServiceTests
{
    [Fact]
    public async Task ShouldAddMeetingViewAsyncAsync()
    {
        // Arrange
        dynamic someMeetingViewInput = CreateRandomMeetingView();

        var someMeetingView = new MeetingView
        {
            Id = someMeetingViewInput.Id,
            Description = someMeetingViewInput.Description,
            Start = someMeetingViewInput.Start
        };

        MeetingView inputMeetingView = someMeetingView;
        MeetingView expectedMeetingView = inputMeetingView;

        var someMeeting = new Meeting
        {
            Id = someMeetingViewInput.Id,
            Description = someMeetingViewInput.Description,
            Start = someMeetingViewInput.Start
        };

        Meeting expectedInputMeeting = someMeeting;
        Meeting returnedMeeting = expectedInputMeeting;

        this.meetingServiceMock.Setup(service =>
            service.AddAsync(It.IsAny<Meeting>()))
                .ReturnsAsync(returnedMeeting);

        // Act
        MeetingView actualMeetingView =
            await this.meetingViewService.AddAsync(inputMeetingView);

        // Assert
        actualMeetingView.Should().BeEquivalentTo(expectedMeetingView);

        this.meetingServiceMock.Verify(service =>
            service.AddAsync(It.Is(SameMeetingAs(expectedInputMeeting))),
            Times.Once);
    }
}