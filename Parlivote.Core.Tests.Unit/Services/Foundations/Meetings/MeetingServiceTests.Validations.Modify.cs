using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Meetings;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnModifyIfMeetingIsNullAndLogItAsync()
    {
        // Arrange
        Meeting nullMeeting = null;

        var nullMeetingException =
            new NullMeetingException();

        var expectedMeetingValidationException =
            new MeetingValidationException(nullMeetingException);

        // Act
        Task<Meeting> modifyMeetingTask = this.meetingService.ModifyAsync(nullMeeting);
         
        // Assert
        await Assert.ThrowsAsync<MeetingValidationException>(() => modifyMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

}