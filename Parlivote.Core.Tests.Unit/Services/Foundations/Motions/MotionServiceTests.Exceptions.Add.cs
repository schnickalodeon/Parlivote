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
    public async Task ThrowsCriticalDependencyException_OnAddIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Motion someMotion = GetRandomMotion();
        Motion inputMotion = someMotion;

        SqlException sqlException = Tests.GetSqlException();

        var pollStorageException =
            new FailedMotionStorageException(sqlException);

        var expectedMotionDependencyException =
            new MotionDependencyException(pollStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()))
                .ThrowsAsync(sqlException);

        // Act
        Task<Motion> addMotionTask =
            this.pollService.AddAsync(inputMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(inputMotion),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(this.loggingBrokerMock, expectedMotionDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionOnAddIfMotionAlreadyExistsAndLogItAsync()
    {
        // Arrange
        Motion randomMotion = GetRandomMotion();
        Motion alreadyExistingMotion = randomMotion;
        string randomMessage = Tests.GetRandomString();

        var duplicateKeyException =
            new DuplicateKeyException(randomMessage);

        var alreadyExistsMotionException =
            new AlreadyExistsMotionException(duplicateKeyException);

        var expectedMotionDependencyValidationException
            = new MotionDependencyValidationException(alreadyExistsMotionException);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()))
                .ThrowsAsync(duplicateKeyException);

        // Act
        Task<Motion> addMotionTask =
            this.pollService.AddAsync(alreadyExistingMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyValidationException>(() => addMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(alreadyExistingMotion),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionDependencyValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
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
            broker.InsertMotionAsync(It.IsAny<Motion>()))
                .ThrowsAsync(databaseUpdateException);

        // Act
        Task<Motion> addMotionTask = this.pollService.AddAsync(someMotion);

        // Assert
        await Assert.ThrowsAsync<MotionDependencyException>(() => addMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(someMotion),
            Times.Once());

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
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

        this.storageBrokerMock.Setup(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Motion> addMotionTask = this.pollService.AddAsync(someMotion);

        //Assert
        await Assert.ThrowsAsync<MotionServiceException>(() => addMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,expectedMotionServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}