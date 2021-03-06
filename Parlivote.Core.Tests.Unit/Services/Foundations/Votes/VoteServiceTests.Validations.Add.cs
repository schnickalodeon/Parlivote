using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Votes;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Parlivote.Shared.Models.VoteValues;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnAddIfVoteIsNullAndLogItAsync()
    {
        // Arrange
        Vote nullVote = null;

        var nullVoteException =
            new NullVoteException();

        var expectedVoteValidationException =
            new VoteValidationException(nullVoteException);

        // Act
        Task<Vote> addVoteTask = this.voteService.AddAsync(nullVote);
         
        // Assert
        await Assert.ThrowsAsync<VoteValidationException>(() => addVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnAddIfVoteIsInvalidAndLogItAsync()
    {
        // Arrange
        var invalidVote = new Vote
        {
            Id = Guid.Empty,
            MotionId = Guid.Empty,
            UserId = Guid.Empty,
            Value = (VoteValue)4
        };

        var invalidVoteException =
            new InvalidVoteException();

        invalidVoteException.AddData(
            key: nameof(Vote.Id),
            values: ExceptionMessages.INVALID_ID);

        invalidVoteException.AddData(
            key: nameof(Vote.MotionId),
            values: ExceptionMessages.INVALID_ID);

        invalidVoteException.AddData(
            key: nameof(Vote.UserId),
            values: ExceptionMessages.INVALID_ID);

        invalidVoteException.AddData(
            key: nameof(Vote.Value),
            values: ExceptionMessages.Vote.INVALID_VALUE);

        var expectedVoteValidationException
            = new VoteValidationException(invalidVoteException);

        // Act
        Task<Vote> addVoteTask = this.voteService.AddAsync(invalidVote);

        // Assert
        await Assert.ThrowsAsync<VoteValidationException>(() => addVoteTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteValidationException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}