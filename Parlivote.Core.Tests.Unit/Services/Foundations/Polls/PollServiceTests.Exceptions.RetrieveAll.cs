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
    public void ThrowsCriticalDependencyException_OnRetrieveAllIfSqlErrorOccursAndLogItAsync()
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
            broker.SelectAllPolls())
                .Throws(sqlException);

        // Act
        Action retrieveAllAction = () => this.pollService.RetrieveAll();

        // Assert
        Assert.Throws<PollDependencyException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPolls(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedPollDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

}