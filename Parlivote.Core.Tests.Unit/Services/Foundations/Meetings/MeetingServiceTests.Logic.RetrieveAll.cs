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
    public void ShouldRetrieveAllMeetings()
    {
        // Arrange
        IQueryable<Meeting> someMeetings = GetRandomMeetings();
        IQueryable<Meeting> storageMeetings = someMeetings;
        IQueryable<Meeting> expectedMeetings = storageMeetings.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllMeetings())
                .Returns(storageMeetings);

        // Act
        IQueryable<Meeting> actualMeetings = this.meetingService.RetrieveAll();
        
        // Assert
        actualMeetings.Should().BeEquivalentTo(expectedMeetings);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllMeetings(),
            Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}