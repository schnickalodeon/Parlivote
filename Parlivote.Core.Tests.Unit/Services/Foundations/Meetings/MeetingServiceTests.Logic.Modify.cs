using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Meetings;

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

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMeeting);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdateMeetingAsync(It.IsAny<Meeting>()))
                .ReturnsAsync(updatedMeeting);

        // Act
        Meeting actualMeeting =
            await this.meetingService.ModifyAsync(inputMeeting);

        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(meetingId),
            Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateMeetingAsync(inputMeeting),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}