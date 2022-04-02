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
    public async Task ShouldDeleteMeetingAsync()
    {
        // Arrange
        Guid randomId = Guid.NewGuid();
        Guid inputId = randomId;
        Meeting someMeeting = GetRandomMeeting();
        Meeting storageMeeting = someMeeting;
        Meeting expectedInputMeeting = storageMeeting;
        Meeting deletedMeeting = expectedInputMeeting;
        Meeting expectedMeeting = deletedMeeting.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMeeting);

        this.storageBrokerMock.Setup(broker =>
            broker.DeleteMeetingAsync(It.IsAny<Meeting>()))
            .ReturnsAsync(deletedMeeting);

        // Act
        Meeting actualMeeting =
            await this.meetingService.DeleteMeetingById(inputId);

        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputId),
            Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteMeetingAsync(expectedInputMeeting),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}