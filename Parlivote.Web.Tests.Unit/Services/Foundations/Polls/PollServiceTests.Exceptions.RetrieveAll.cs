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
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Fact]
    public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfResponseInternalServerExceptionOccursAndLogItAsync()
    {
        // Arrange
        string someMessage = Tests.GetRandomString();
        var httpResponseMessage = new HttpResponseMessage();

        var httpResponseException =
            new HttpResponseException(
                httpResponseMessage,
                someMessage);

        var failedPollDependencyException =
            new FailedPollDependencyException(httpResponseException);

        var expectedPollDependencyException =
            new PollDependencyException(failedPollDependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllPollsAsync())
                .ThrowsAsync(httpResponseException);

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
}