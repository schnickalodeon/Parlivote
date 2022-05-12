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
    public void ThrowsCriticalDependencyException_OnRetrieveAllIfSqlErrorOccursAndLogItAsync()
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
            broker.SelectAllVotes())
                .Throws(sqlException);

        // Act
        Action retrieveAllAction = () => this.voteService.RetrieveAll();

        // Assert
        Assert.Throws<VoteDependencyException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllVotes(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
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
            broker.SelectAllVotes())
            .Throws(serviceException);

        // Act
        Action retrieveAllAction = () => this.voteService.RetrieveAll();

        // Assert
        Assert.Throws<VoteServiceException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllVotes(),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedVoteServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

}