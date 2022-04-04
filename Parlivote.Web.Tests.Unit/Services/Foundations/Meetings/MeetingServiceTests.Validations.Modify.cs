using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Meetings;

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

        this.apiBrokerMock.Verify(broker =>
            broker.PutMeetingAsync(It.IsAny<Meeting>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnModifyIfMeetingIsInvalidAndLogItAsync(string invalidText)
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
        Task<Meeting> modifyMeetingTask = this.meetingService.ModifyAsync(invalidMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingValidationException>(() => modifyMeetingTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingValidationException);

        this.apiBrokerMock.Verify(broker =>
            broker.PutMeetingAsync(It.IsAny<Meeting>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfMeetingDoesNotExistAndLogItAsync()
    {
        // given
        Meeting randomMeeting = GetRandomMeeting();
        Meeting nonExistMeeting = randomMeeting;
        Meeting nullMeeting = null;

        var notFoundMeetingException =
            new NotFoundMeetingException(nonExistMeeting.Id);

        var expectedMeetingValidationException =
            new MeetingValidationException(notFoundMeetingException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMeetingById(nonExistMeeting.Id))
                .ReturnsAsync(nullMeeting);

        // when 
        Task<Meeting> modifyMeetingTask =
            this.meetingService.ModifyAsync(nonExistMeeting);

        // then
        await Assert.ThrowsAsync<MeetingValidationException>(() => modifyMeetingTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(nonExistMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedMeetingValidationException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}