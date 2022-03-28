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
    public async Task ShouldAddMeetingAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting inputMeeting = someMeeting;
        Meeting storageMeeting = inputMeeting;
        Meeting expectedMeeting = storageMeeting.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.PostMeetingAsync(It.IsAny<Meeting>()))
                .ReturnsAsync(storageMeeting);

        // Act
        Meeting actualMeeting =
            await this.meetingService.AddAsync(inputMeeting);

        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.apiBrokerMock.Verify(broker =>
            broker.PostMeetingAsync(inputMeeting),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}