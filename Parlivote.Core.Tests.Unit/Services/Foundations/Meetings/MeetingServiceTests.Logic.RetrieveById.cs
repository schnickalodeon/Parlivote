using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task ShouldRetrieveMeetingsByIdAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting storageMeeting = someMeeting;
        Meeting expectedMeeting = someMeeting.DeepClone();
        Guid inputMeetingId = someMeeting.Id;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ReturnsAsync(storageMeeting);

        // Act
        Meeting actualMeeting = 
            await this.meetingService.RetrieveByIdAsync(inputMeetingId);
        
        // Assert
        actualMeeting.Should().BeEquivalentTo(expectedMeeting);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputMeetingId),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}