using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ThrowsCriticalDependencyException_OnModifyIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        Motion inputMotion = someMotion;

        SqlException sqlException = Tests.GetSqlException();

        var motionStorageException = 
            new FailedMotionStorageException(sqlException);

        var expectedMotionDependencyException =
            new MotionDependencyException(motionStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Motion> addMotionTask =
            this.motionService.ModifyAsync(inputMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(inputMotion.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();

        var databaseUpdateException =
            new DbUpdateException();

        var failedMotionStorageException =
            new FailedMotionStorageException(databaseUpdateException);

        var expectedDependencyException =
            new MotionDependencyException(failedMotionStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Motion> modifyMotionTask = this.motionService.ModifyAsync(someMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => modifyMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(someMotion.Id),
            Times.Once());

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowAndLogDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccurs()
    {
        // Arrange
        Motion randomMotion = GetRandomMotion();
        Motion inputMotion = randomMotion;

        var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

        var lockedMotionException = 
            new LockedMotionException(databaseUpdateConcurrencyException);

        var expectedMotionDependencyValidationException =
            new MotionDependencyValidationException(lockedMotionException);

        this.storageBrokerMock.Setup(broker => 
            broker.SelectMotionById(It.IsAny<Guid>()))
            .ThrowsAsync(databaseUpdateConcurrencyException);

        // Act
        Task<Motion> modifyMotionTask =
            this.motionService.ModifyAsync(inputMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyValidationException>(() => modifyMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(inputMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedMotionDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
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

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Motion> modifyMotionTask = this.motionService.ModifyAsync(someMotion);

        //Assert
        await Assert.ThrowsAsync<MotionServiceException>(() => modifyMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(someMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedMotionServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}