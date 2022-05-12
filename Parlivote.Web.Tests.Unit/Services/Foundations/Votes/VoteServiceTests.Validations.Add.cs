using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Parlivote.Shared.Models.VoteValues;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Votes;

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

        this.apiBrokerMock.Verify(broker =>
            broker.PostVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedVoteValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnAddIfVoteIsInvalidAndLogItAsync(string invalidText)
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

        this.apiBrokerMock.Verify(broker =>
            broker.PostVoteAsync(It.IsAny<Vote>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

}