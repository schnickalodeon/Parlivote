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
    public async Task ShouldThrowValidationException_OnAddIfMeetingIdIsInvalidAndLogItAsync()
    {
        // Arrange
        Guid invalidMeetingId = Guid.Empty;

        var invalidMeetingException =
            new InvalidMeetingException();

        invalidMeetingException.AddData(
            key: nameof(Meeting.Id),
            values: ExceptionMessages.INVALID_ID);

        var expectedMeetingValidationException =
            new MeetingValidationException(invalidMeetingException);

        // Act
        Task<Meeting> retrieveByIdAsyncTask =
            this.meetingService.RetrieveByIdAsync(invalidMeetingId);
         
        // Assert
        await Assert.ThrowsAsync<MeetingValidationException>(() => retrieveByIdAsyncTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}