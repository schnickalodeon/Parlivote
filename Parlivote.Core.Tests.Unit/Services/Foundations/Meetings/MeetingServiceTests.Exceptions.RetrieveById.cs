using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Guid inputMeetingId = Guid.NewGuid();
        SqlException sqlException = Tests.GetSqlException();

        var meetingStorageException =
            new FailedMeetingStorageException(sqlException);

        var expectedMeetingDependencyException =
            new MeetingDependencyException(meetingStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .Throws(sqlException);

        // Act
        Task<Meeting> retrieveByIdTask = 
            this.meetingService.RetrieveByIdAsync(inputMeetingId);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputMeetingId),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid inputMeetingId = Guid.NewGuid();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedMeetingServiceException =
            new FailedMeetingServiceException(serviceException);

        var expectedMeetingServiceException =
            new MeetingServiceException(failedMeetingServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .Throws(serviceException);

        // Act
        Task<Meeting> retrieveByIdTask =
            this.meetingService.RetrieveByIdAsync(inputMeetingId);

        // Assert
        await Assert.ThrowsAsync<MeetingServiceException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputMeetingId),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}