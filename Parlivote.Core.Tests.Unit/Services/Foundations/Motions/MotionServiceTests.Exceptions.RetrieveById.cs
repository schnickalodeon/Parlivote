using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Guid inputMotionId = Guid.NewGuid();
        SqlException sqlException = Tests.GetSqlException();

        var motionStorageException =
            new FailedMotionStorageException(sqlException);

        var expectedMotionDependencyException =
            new MotionDependencyException(motionStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .Throws(sqlException);

        // Act
        Task<Motion> retrieveByIdTask = 
            this.motionService.RetrieveByIdAsync(inputMotionId);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(inputMotionId),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid inputMotionId = Guid.NewGuid();
        string randomExceptionMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomExceptionMessage);

        var failedMotionServiceException =
            new FailedMotionServiceException(serviceException);

        var expectedMotionServiceException =
            new MotionServiceException(failedMotionServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .Throws(serviceException);

        // Act
        Task<Motion> retrieveByIdTask =
            this.motionService.RetrieveByIdAsync(inputMotionId);

        // Assert
        await Assert.ThrowsAsync<MotionServiceException>(() => retrieveByIdTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(inputMotionId),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}