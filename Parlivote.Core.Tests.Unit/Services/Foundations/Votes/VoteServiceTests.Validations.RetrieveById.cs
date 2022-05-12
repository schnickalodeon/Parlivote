using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Votes;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnAddIfVoteIdIsInvalidAndLogItAsync()
    {
        // Arrange
        Guid invalidVoteId = Guid.Empty;

        var invalidVoteException =
            new InvalidVoteException();

        invalidVoteException.AddData(
            key: nameof(Vote.Id),
            values: ExceptionMessages.INVALID_ID);

        var expectedVoteValidationException =
            new VoteValidationException(invalidVoteException);

        // Act
        Task<Vote> retrieveByIdAsyncTask =
            this.voteService.RetrieveByIdAsync(invalidVoteId);
         
        // Assert
        await Assert.ThrowsAsync<VoteValidationException>(() => retrieveByIdAsyncTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(It.IsAny<Guid>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}