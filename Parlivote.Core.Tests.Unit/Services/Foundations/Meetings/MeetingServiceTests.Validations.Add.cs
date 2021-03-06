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
    public async Task ShouldThrowValidationException_OnAddIfMeetingIsNullAndLogItAsync()
    {
        // Arrange
        Meeting nullMeeting = null;

        var nullMeetingException =
            new NullMeetingException();

        var expectedMeetingValidationException =
            new MeetingValidationException(nullMeetingException);

        // Act
        Task<Meeting> addMeetingTask = this.meetingService.AddAsync(nullMeeting);
         
        // Assert
        await Assert.ThrowsAsync<MeetingValidationException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnAddIfMeetingIsInvalidAndLogItAsync(string invalidText)
    {
        // Arrange
        var invalidMeeting = new Meeting
        {
            Id = Guid.Empty,
            Description = invalidText
        };

        var invalidMeetingException =
            new InvalidMeetingException();

        invalidMeetingException.AddData(
            key: nameof(Meeting.Id),
            values: ExceptionMessages.INVALID_ID);

        invalidMeetingException.AddData(
            key: nameof(Meeting.Description),
            values: ExceptionMessages.INVALID_STRING);

        var expectedMeetingValidationException
            = new MeetingValidationException(invalidMeetingException);

        // Act
        Task<Meeting> addMeetingTask = this.meetingService.AddAsync(invalidMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingValidationException>(() => addMeetingTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingValidationException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}