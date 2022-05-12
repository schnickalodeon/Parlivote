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
using Xunit.Sdk;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnModifyIfVoteIsNullAndLogItAsync()
    {
        // Arrange
        Vote nullVote = null;

        var nullVoteException =
            new NullVoteException();

        var expectedVoteValidationException =
            new VoteValidationException(nullVoteException);

        // Act
        Task<Vote> modifyVoteTask = this.voteService.ModifyAsync(nullVote);
         
        // Assert
        await Assert.ThrowsAsync<VoteValidationException>(() => modifyVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnModifyIfVoteIsInvalidAndLogItAsync(string invalidText)
    {
        // Arrange
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
        Task<Vote> modifyVoteTask = this.voteService.ModifyAsync(invalidVote);

        // Assert
        await Assert.ThrowsAsync<VoteValidationException>(() => modifyVoteTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteValidationException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfVoteDoesNotExistAndLogItAsync()
    {
        // given
        Vote randomVote = GetRandomVote();
        Vote nonExistVote = randomVote;
        Vote nullVote = null;

        var notFoundVoteException =
            new NotFoundVoteException(nonExistVote.Id);

        var expectedVoteValidationException =
            new VoteValidationException(notFoundVoteException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectVoteById(nonExistVote.Id))
                .ReturnsAsync(nullVote);

        // when 
        Task<Vote> modifyVoteTask =
            this.voteService.ModifyAsync(nonExistVote);

        // then
        await Assert.ThrowsAsync<VoteValidationException>(() => modifyVoteTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectVoteById(nonExistVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedVoteValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}