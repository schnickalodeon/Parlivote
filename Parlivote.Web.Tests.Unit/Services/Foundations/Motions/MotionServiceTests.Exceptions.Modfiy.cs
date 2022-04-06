using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Theory]
    [MemberData(nameof(DependencyValidationException))]
    public async Task ShouldThrowAndLogDependencyValidationException_OnModify_IfDependencyValidationExceptionOccurs(
           Xeption dependencyValidationException)
    {
        // Arrange
        Motion someMotion = GetRandomMotion();

        var expectedMotionDependencyValidationException =
            new MotionDependencyValidationException(
                dependencyValidationException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyValidationException);

        // Act
        Task<Motion> modifyMotionTask =
            this.motionService.ModifyAsync(someMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyValidationException>(() => modifyMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMotionById(someMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyValidationException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(CriticalDependencyException))]
    public async Task ShouldThrowAndLogCriticalDependencyException_OnModify_IfCriticalExceptionOccurs(
        Xeption criticalException)
    {
        // Arrange
        Motion someMotion = GetRandomMotion();

        var expectedMotionDependencyException =
            new MotionDependencyException(
                criticalException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(criticalException);

        // Act
        Task<Motion> modifyMotionTask =
            this.motionService.ModifyAsync(someMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => modifyMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMotionById(someMotion.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(DependencyException))]
    public async Task ShouldThrowAndLogDependencyException_OnModify_IfDependencyExceptionErrorOccurs(
        Xeption dependencyException)
    {
        // Arrange
        Motion someMotion = GetRandomMotion();

        var expectedMotionDependencyException =
            new MotionDependencyException(
                dependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyException);

        // Act
        Task<Motion> modifyMotionTask =
            this.motionService.ModifyAsync(someMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => modifyMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMotionById(someMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
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
            broker.GetMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Motion> modifyMotionTask = this.motionService.ModifyAsync(someMotion);

        //Assert
        await Assert.ThrowsAsync<MotionServiceException>(() => modifyMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMotionById(someMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedMotionServiceException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}