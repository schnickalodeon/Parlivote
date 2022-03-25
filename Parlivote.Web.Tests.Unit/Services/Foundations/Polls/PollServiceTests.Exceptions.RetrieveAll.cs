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
            broker.PostPollAsync(It.IsAny<Poll>()))
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
}