using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Guid inputVoteId = Guid.NewGuid();
        SqlException sqlException = Tests.GetSqlException();

        var voteStorageException =
            new FailedVoteStorageException(sqlException);

        var expectedVoteDependencyException =
            new VoteDependencyException(voteStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectVoteById(It.IsAny<Guid>()))
                .Throws(sqlException);

        // Act
        Task<Vote> retrieveByIdTask = 
            this.voteService.RetrieveByIdAsync(inputVoteId);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(inputVoteId),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid inputVoteId = Guid.NewGuid();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedVoteServiceException =
            new FailedVoteServiceException(serviceException);

        var expectedVoteServiceException =
            new VoteServiceException(failedVoteServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectVoteById(It.IsAny<Guid>()))
                .Throws(serviceException);

        // Act
        Task<Vote> retrieveByIdTask =
            this.voteService.RetrieveByIdAsync(inputVoteId);

        // Assert
        await Assert.ThrowsAsync<VoteServiceException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(inputVoteId),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}