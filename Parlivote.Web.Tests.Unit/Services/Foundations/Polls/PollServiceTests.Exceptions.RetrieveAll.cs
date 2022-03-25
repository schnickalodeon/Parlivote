using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Theory]
    [MemberData(nameof(DependencyException))]
    public async Task ShouldThrowAndLogDependencyException_OnRetrieveAll_IfDependencyExceptionErrorOccurs(
        Xeption dependencyException)
    {
        // Arrange
        var expectedPollDependencyException =
            new PollDependencyException(
                dependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllPollsAsync())
            .ThrowsAsync(dependencyException);

        // Act
        Task<List<Poll>> addPollTask =
            this.pollService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllPollsAsync(),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedPollDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(CriticalDependencyException))]
    public async Task ShouldThrowAndLogCriticalDependencyException_OnRetrieveAll_IfCriticalExceptionOccurs(
        Xeption criticalException)
    {
        // Arrange
        var expectedPollDependencyException =
            new PollDependencyException(
                criticalException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllPollsAsync())
            .ThrowsAsync(criticalException);

        // Act
        Task<List<Poll>> retrieveAllTask = this.pollService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<PollDependencyException>(() => retrieveAllTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllPollsAsync(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedPollDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowAndLogServiceException_OnRetrieveAll_IfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Poll somePoll = GetRandomPoll();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedPollServiceException =
            new FailedPollServiceException(serviceException);

        var expectedPollServiceException =
            new PollServiceException(failedPollServiceException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllPollsAsync())
                .ThrowsAsync(serviceException);

        //Act
        Task<List<Poll>> retrieveAllTask = this.pollService.RetrieveAllAsync();

        //Assert
        await Assert.ThrowsAsync<PollServiceException>(() => retrieveAllTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllPollsAsync(),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedPollServiceException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}