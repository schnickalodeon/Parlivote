using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
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
            this.pollService.AddPollAsync(inputPoll);

        // Assert
        await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(inputPoll),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(this.loggingBrokerMock, expectedPollDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}