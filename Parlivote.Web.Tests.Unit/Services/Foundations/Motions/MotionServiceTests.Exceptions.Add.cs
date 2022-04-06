using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions
{
    public partial class MotionServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationException))]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAdd_IfDependencyValidationExceptionOccurs(
            Xeption dependencyValidationException)
        {
            // Arrange
            Motion someMotion = GetRandomMotion();

            var expectedMotionDependencyValidationException =
                new MotionDependencyValidationException(
                    dependencyValidationException);

            this.apiBrokerMock.Setup(broker =>
                    broker.PostMotionAsync(It.IsAny<Motion>()))
                    .ThrowsAsync(dependencyValidationException);

            // Act
            Task<Motion> addMotionTask =
                this.motionService.AddAsync(someMotion);

            // Assert
            await Assert.ThrowsAsync<MotionDependencyValidationException>(() => addMotionTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMotionAsync(someMotion),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedMotionDependencyValidationException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(CriticalDependencyException))]
        public async Task ShouldThrowAndLogCriticalDependencyException_OnAdd_IfCriticalExceptionOccurs(
            Xeption criticalException)
        {
            // Arrange
            Motion someMotion = GetRandomMotion();

            var expectedMotionDependencyException =
                new MotionDependencyException(
                    criticalException);

            this.apiBrokerMock.Setup(broker =>
                    broker.PostMotionAsync(It.IsAny<Motion>()))
                    .ThrowsAsync(criticalException);

            // Act
            Task<Motion> addMotionTask =
                this.motionService.AddAsync(someMotion);

            // Assert
            await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMotionAsync(someMotion),
                Times.Once);

            Tests.VerifyCriticalExceptionLogged(
                this.loggingBrokerMock,
                expectedMotionDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyException))]
        public async Task ShouldThrowAndLogDependencyException_OnAdd_IfDependencyExceptionErrorOccurs(
            Xeption dependencyException)
        {
            // Arrange
            Motion someMotion = GetRandomMotion();

            var expectedMotionDependencyException =
                new MotionDependencyException(
                    dependencyException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostMotionAsync(It.IsAny<Motion>()))
                    .ThrowsAsync(dependencyException);

            // Act
            Task<Motion> addMotionTask =
                this.motionService.AddAsync(someMotion);

            // Assert
            await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMotionAsync(someMotion),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedMotionDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
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
                    broker.PostMotionAsync(It.IsAny<Motion>()))
                    .ThrowsAsync(serviceException);

            //Act
            Task<Motion> addMotionTask = this.motionService.AddAsync(someMotion);

            //Assert
            await Assert.ThrowsAsync<MotionServiceException>(() => addMotionTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMotionAsync(It.IsAny<Motion>()),
                Times.Once);

            Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedMotionServiceException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
