using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnAddIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Vote someVote = GetRandomVote();
        Vote inputVote = someVote;

        SqlException sqlException = Tests.GetSqlException();

        var voteStorageException =
            new FailedVoteStorageException(sqlException);

        var expectedVoteDependencyException =
            new VoteDependencyException(voteStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Vote> addVoteTask =
            this.voteService.AddAsync(inputVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(inputVote),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(this.loggingBrokerMock, expectedVoteDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionOnAddIfVoteAlreadyExistsAndLogItAsync()
    {
        // Arrange
        Vote randomVote = GetRandomVote();
        Vote alreadyExistingVote = randomVote;
        string randomMessage = Tests.GetRandomString();

        var duplicateKeyException =
            new DuplicateKeyException(randomMessage);

        var alreadyExistsVoteException =
            new AlreadyExistsVoteException(duplicateKeyException);

        var expectedVoteDependencyValidationException
            = new VoteDependencyValidationException(alreadyExistsVoteException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()))
                .ThrowsAsync(duplicateKeyException);

        // Act
        Task<Vote> addVoteTask =
            this.voteService.AddAsync(alreadyExistingVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyValidationException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(alreadyExistingVote),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // Arrange
        Vote someVote = GetRandomVote();

        var databaseUpdateException =
            new DbUpdateException();

        var failedVoteStorageException =
            new FailedVoteStorageException(databaseUpdateException);

        var expectedDependencyException =
            new VoteDependencyException(failedVoteStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Vote> addVoteTask = this.voteService.AddAsync(someVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(someVote),
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
        Vote someVote = GetRandomVote();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedVoteServiceException =
            new FailedVoteServiceException(serviceException);

        var expectedVoteServiceException =
            new VoteServiceException(failedVoteServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Vote> addVoteTask = this.voteService.AddAsync(someVote);

        //Assert
        await Assert.ThrowsAsync<VoteServiceException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,expectedVoteServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}