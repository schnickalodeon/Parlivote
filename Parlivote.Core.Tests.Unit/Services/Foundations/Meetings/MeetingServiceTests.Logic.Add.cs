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
    public async Task ShouldAddMeetingAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting inputMeeting = someMeeting;
        Meeting storageMeeting = inputMeeting;
        Meeting expectedMeeting = storageMeeting.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()))
                .ReturnsAsync(storageMeeting);

        // Act
        Meeting actualMeeting =
            await this.meetingService.AddAsync(inputMeeting);

        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(inputMeeting),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}