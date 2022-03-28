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
    public async Task ThrowsCriticalDependencyException_OnAddIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting inputMeeting = someMeeting;

        SqlException sqlException = Tests.GetSqlException();

        var meetingStorageException =
            new FailedMeetingStorageException(sqlException);

        var expectedMeetingDependencyException =
            new MeetingDependencyException(meetingStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Meeting> addMeetingTask =
            this.meetingService.AddAsync(inputMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(inputMeeting),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(this.loggingBrokerMock, expectedMeetingDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionOnAddIfMeetingAlreadyExistsAndLogItAsync()
    {
        // Arrange
        Meeting randomMeeting = GetRandomMeeting();
        Meeting alreadyExistingMeeting = randomMeeting;
        string randomMessage = Tests.GetRandomString();

        var duplicateKeyException =
            new DuplicateKeyException(randomMessage);

        var alreadyExistsMeetingException =
            new AlreadyExistsMeetingException(duplicateKeyException);

        var expectedMeetingDependencyValidationException
            = new MeetingDependencyValidationException(alreadyExistsMeetingException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()))
                .ThrowsAsync(duplicateKeyException);

        // Act
        Task<Meeting> addMeetingTask =
            this.meetingService.AddAsync(alreadyExistingMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyValidationException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(alreadyExistingMeeting),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();

        var databaseUpdateException =
            new DbUpdateException();

        var failedMeetingStorageException =
            new FailedMeetingStorageException(databaseUpdateException);

        var expectedDependencyException =
            new MeetingDependencyException(failedMeetingStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Meeting> addMeetingTask = this.meetingService.AddAsync(someMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(someMeeting),
            Times.Once());

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedMeetingServiceException =
            new FailedMeetingServiceException(serviceException);

        var expectedMeetingServiceException =
            new MeetingServiceException(failedMeetingServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Meeting> addMeetingTask = this.meetingService.AddAsync(someMeeting);

        //Assert
        await Assert.ThrowsAsync<MeetingServiceException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMeetingAsync(It.IsAny<Meeting>()),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,expectedMeetingServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}