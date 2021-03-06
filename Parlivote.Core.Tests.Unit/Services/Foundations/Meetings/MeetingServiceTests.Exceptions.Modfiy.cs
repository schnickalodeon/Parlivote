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
    public async Task ThrowsCriticalDependencyException_OnModifyIfSqlErrorOccursAndLogItAsync()
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
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Meeting> addMeetingTask =
            this.meetingService.ModifyAsync(inputMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => addMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputMeeting.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
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
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Meeting> modifyMeetingTask = this.meetingService.ModifyAsync(someMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => modifyMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(someMeeting.Id),
            Times.Once());

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowAndLogDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccurs()
    {
        // Arrange
        Meeting randomMeeting = GetRandomMeeting();
        Meeting inputMeeting = randomMeeting;

        var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

        var lockedMeetingException = 
            new LockedMeetingException(databaseUpdateConcurrencyException);

        var expectedMeetingDependencyValidationException =
            new MeetingDependencyValidationException(lockedMeetingException);

        this.storageBrokerMock.Setup(broker => 
            broker.SelectMeetingById(It.IsAny<Guid>()))
            .ThrowsAsync(databaseUpdateConcurrencyException);

        // Act
        Task<Meeting> modifyMeetingTask =
            this.meetingService.ModifyAsync(inputMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyValidationException>(() => modifyMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(inputMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedMeetingDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
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
            broker.SelectMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Meeting> modifyMeetingTask = this.meetingService.ModifyAsync(someMeeting);

        //Assert
        await Assert.ThrowsAsync<MeetingServiceException>(() => modifyMeetingTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMeetingById(someMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedMeetingServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}