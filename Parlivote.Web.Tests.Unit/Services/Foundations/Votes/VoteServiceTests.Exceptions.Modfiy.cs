using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    [Theory]
    [MemberData(nameof(DependencyValidationException))]
    public async Task ShouldThrowAndLogDependencyValidationException_OnModify_IfDependencyValidationExceptionOccurs(
           Xeption dependencyValidationException)
    {
        // Arrange
        Vote someVote = GetRandomVote();

        var expectedVoteDependencyValidationException =
            new VoteDependencyValidationException(
                dependencyValidationException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyValidationException);

        // Act
        Task<Vote> modifyVoteTask =
            this.voteService.ModifyAsync(someVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyValidationException>(() => modifyVoteTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetVoteById(someVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteDependencyValidationException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(CriticalDependencyException))]
    public async Task ShouldThrowAndLogCriticalDependencyException_OnModify_IfCriticalExceptionOccurs(
        Xeption criticalException)
    {
        // Arrange
        Vote someVote = GetRandomVote();

        var expectedVoteDependencyException =
            new VoteDependencyException(
                criticalException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(criticalException);

        // Act
        Task<Vote> modifyVoteTask =
            this.voteService.ModifyAsync(someVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => modifyVoteTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetVoteById(someVote.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(DependencyException))]
    public async Task ShouldThrowAndLogDependencyException_OnModify_IfDependencyExceptionErrorOccurs(
        Xeption dependencyException)
    {
        // Arrange
        Vote someVote = GetRandomVote();

        var expectedVoteDependencyException =
            new VoteDependencyException(
                dependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyException);

        // Act
        Task<Vote> modifyVoteTask =
            this.voteService.ModifyAsync(someVote);

        // Assert
        await Assert.ThrowsAsync<VoteDependencyException>(() => modifyVoteTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetVoteById(someVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedVoteDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
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

        this.apiBrokerMock.Setup(broker =>
            broker.GetVoteById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Vote> modifyVoteTask = this.voteService.ModifyAsync(someVote);

        //Assert
        await Assert.ThrowsAsync<VoteServiceException>(() => modifyVoteTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetVoteById(someVote.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedVoteServiceException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}