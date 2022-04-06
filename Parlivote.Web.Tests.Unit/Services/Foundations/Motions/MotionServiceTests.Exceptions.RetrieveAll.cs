using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Theory]
    [MemberData(nameof(DependencyException))]
    public async Task ShouldThrowAndLogDependencyException_OnRetrieveAll_IfDependencyExceptionErrorOccurs(
        Xeption dependencyException)
    {
        // Arrange
        var expectedMotionDependencyException =
            new MotionDependencyException(
                dependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllMotionsAsync())
            .ThrowsAsync(dependencyException);

        // Act
        Task<List<Motion>> addMotionTask =
            this.motionService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllMotionsAsync(),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(CriticalDependencyException))]
    public async Task ShouldThrowAndLogCriticalDependencyException_OnRetrieveAll_IfCriticalExceptionOccurs(
        Xeption criticalException)
    {
        // Arrange
        var expectedMotionDependencyException =
            new MotionDependencyException(
                criticalException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllMotionsAsync())
            .ThrowsAsync(criticalException);

        // Act
        Task<List<Motion>> retrieveAllTask = this.motionService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => retrieveAllTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllMotionsAsync(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowAndLogServiceException_OnRetrieveAll_IfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedMotionServiceException =
            new FailedMotionServiceException(serviceException);

        var expectedMotionServiceException =
            new MotionServiceException(failedMotionServiceException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllMotionsAsync())
                .ThrowsAsync(serviceException);

        //Act
        Task<List<Motion>> retrieveAllTask = this.motionService.RetrieveAllAsync();

        //Assert
        await Assert.ThrowsAsync<MotionServiceException>(() => retrieveAllTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllMotionsAsync(),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionServiceException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}