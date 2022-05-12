using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Votes
{
    public partial class VoteServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationException))]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAdd_IfDependencyValidationExceptionOccurs(
            Xeption dependencyValidationException)
        {
            // Arrange
            Vote someVote = GetRandomVote();

            var expectedVoteDependencyValidationException =
                new VoteDependencyValidationException(
                    dependencyValidationException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostVoteAsync(It.IsAny<Vote>()))
                    .ThrowsAsync(dependencyValidationException);

            // Act
            Task<Vote> addVoteTask =
                this.voteService.AddAsync(someVote);

            // Assert
            await Assert.ThrowsAsync<VoteDependencyValidationException>(() => addVoteTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostVoteAsync(someVote),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedVoteDependencyValidationException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(CriticalDependencyException))]
        public async Task ShouldThrowAndLogCriticalDependencyException_OnAdd_IfCriticalExceptionOccurs(
            Xeption criticalException)
        {
            // Arrange
            Vote someVote = GetRandomVote();

            var expectedVoteDependencyException =
                new VoteDependencyException(
                    criticalException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostVoteAsync(It.IsAny<Vote>()))
                    .ThrowsAsync(criticalException);

            // Act
            Task<Vote> addVoteTask =
                this.voteService.AddAsync(someVote);

            // Assert
            await Assert.ThrowsAsync<VoteDependencyException>(() => addVoteTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostVoteAsync(someVote),
                Times.Once);

            Tests.VerifyCriticalExceptionLogged(
                this.loggingBrokerMock,
                expectedVoteDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Theory]
        [MemberData(nameof(DependencyException))]
        public async Task ShouldThrowAndLogDependencyException_OnAdd_IfDependencyExceptionErrorOccurs(
            Xeption dependencyException)
        {
            // Arrange
            Vote someVote = GetRandomVote();

            var expectedVoteDependencyException =
                new VoteDependencyException(
                    dependencyException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostVoteAsync(It.IsAny<Vote>()))
                    .ThrowsAsync(dependencyException);

            // Act
            Task<Vote> addVoteTask =
                this.voteService.AddAsync(someVote);

            // Assert
            await Assert.ThrowsAsync<VoteDependencyException>(() => addVoteTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostVoteAsync(someVote),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedVoteDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
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
                broker.PostVoteAsync(It.IsAny<Vote>()))
                    .ThrowsAsync(serviceException);

            //Act
            Task<Vote> addVoteTask = this.voteService.AddAsync(someVote);

            //Assert
            await Assert.ThrowsAsync<VoteServiceException>(() => addVoteTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostVoteAsync(It.IsAny<Vote>()),
                Times.Once);

            Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedVoteServiceException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
