using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    [Fact]
    public async Task ShouldModifyMeetingAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting inputMeeting = someMeeting;
        Meeting storageMeeting = inputMeeting.DeepClone();
        Meeting updatedMeeting = inputMeeting;
        Meeting expectedMeeting = updatedMeeting.DeepClone();
        Guid meetingId = inputMeeting.Id;

        this.apiBrokerMock.Setup(broker =>
            broker.GetMeetingById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMeeting);

        this.apiBrokerMock.Setup(broker =>
            broker.PutMeetingAsync(It.IsAny<Meeting>()))
                .ReturnsAsync(updatedMeeting);

        // Act
        Meeting actualMeeting =
            await this.meetingService.ModifyAsync(inputMeeting);

        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(meetingId),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.PutMeetingAsync(inputMeeting),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}