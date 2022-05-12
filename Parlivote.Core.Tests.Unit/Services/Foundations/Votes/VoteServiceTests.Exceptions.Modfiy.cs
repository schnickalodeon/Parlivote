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
    public async Task ThrowsCriticalDependencyException_OnModifyIfSqlErrorOccursAndLogItAsync()
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
            broker.SelectVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Vote> addVoteTask =
            this.voteService.ModifyAsync(inputVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(inputVote.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
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
            broker.SelectVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Vote> modifyVoteTask = this.voteService.ModifyAsync(someVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => modifyVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(someVote.Id),
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
        Vote randomVote = GetRandomVote();
        Vote inputVote = randomVote;

        var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

        var lockedVoteException = 
            new LockedVoteException(databaseUpdateConcurrencyException);

        var expectedVoteDependencyValidationException =
            new VoteDependencyValidationException(lockedVoteException);

        this.storageBrokerMock.Setup(broker => 
            broker.SelectVoteById(It.IsAny<Guid>()))
            .ThrowsAsync(databaseUpdateConcurrencyException);

        // Act
        Task<Vote> modifyVoteTask =
            this.voteService.ModifyAsync(inputVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyValidationException>(() => modifyVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(inputVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedVoteDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
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
            broker.SelectVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Vote> modifyVoteTask = this.voteService.ModifyAsync(someVote);

        //Assert
        await Assert.ThrowsAsync<VoteServiceException>(() => modifyVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(someVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedVoteServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}