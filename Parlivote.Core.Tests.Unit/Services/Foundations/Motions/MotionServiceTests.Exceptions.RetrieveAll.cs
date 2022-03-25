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
    public void ThrowsCriticalDependencyException_OnRetrieveAllIfSqlErrorOccursAndLogItAsync()
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
            broker.SelectAllMotions())
                .Throws(sqlException);

        // Act
        Action retrieveAllAction = () => this.pollService.RetrieveAll();

        // Assert
        Assert.Throws<MotionDependencyException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllMotions(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
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
            broker.SelectAllMotions())
            .Throws(serviceException);

        // Act
        Action retrieveAllAction = () => this.pollService.RetrieveAll();

        // Assert
        Assert.Throws<MotionServiceException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllMotions(),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedMotionServiceException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

}