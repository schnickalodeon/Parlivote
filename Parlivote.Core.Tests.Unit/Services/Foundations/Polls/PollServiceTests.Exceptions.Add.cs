using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnAddIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Poll somePoll = GetRandomPoll();
        Poll inputPoll = somePoll;

        SqlException sqlException = Tests.GetSqlException();

        var pollStorageException =
            new FailedPollStorageException(sqlException);

        var expectedPollDependencyException =
            new PollDependencyException(pollStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Poll> addPollTask =
            this.pollService.AddAsync(inputPoll);

        // Assert
        await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(inputPoll),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(this.loggingBrokerMock, expectedPollDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionOnAddIfPollAlreadyExistsAndLogItAsync()
    {
        // Arrange
        Poll randomPoll = GetRandomPoll();
        Poll alreadyExistingPoll = randomPoll;
        string randomMessage = Tests.GetRandomString();

        var duplicateKeyException =
            new DuplicateKeyException(randomMessage);

        var alreadyExistsPollException =
            new AlreadyExistsPollException(duplicateKeyException);

        var expectedPollDependencyValidationException
            = new PollDependencyValidationException(alreadyExistsPollException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()))
                .ThrowsAsync(duplicateKeyException);

        // Act
        Task<Poll> addPollTask =
            this.pollService.AddAsync(alreadyExistingPoll);

        // Assert
        await Assert.ThrowsAsync<PollDependencyValidationException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(alreadyExistingPoll),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedPollDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // Arrange
        Poll somePoll = GetRandomPoll();

        var databaseUpdateException =
            new DbUpdateException();

        var failedPollStorageException =
            new FailedPollStorageException(databaseUpdateException);

        var expectedDependencyException =
            new PollDependencyException(failedPollStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Poll> addPollTask = this.pollService.AddAsync(somePoll);

        // Assert
        await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(somePoll),
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
        Poll somePoll = GetRandomPoll();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedPollServiceException =
            new FailedPollServiceException(serviceException);

        var expectedPollServiceException =
            new PollServiceException(failedPollServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Poll> addPollTask = this.pollService.AddAsync(somePoll);

        //Assert
        await Assert.ThrowsAsync<PollServiceException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,expectedPollServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}