using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnAddIfMotionIdIsInvalidAndLogItAsync()
    {
        // Arrange
        Guid invalidMotionId = Guid.Empty;

        var invalidMotionException =
            new InvalidMotionException();

        invalidMotionException.AddData(
            key: nameof(Motion.Id),
            values: ExceptionMessages.INVALID_ID);

        var expectedMotionValidationException =
            new MotionValidationException(invalidMotionException);

        // Act
        Task<Motion> retrieveByIdAsyncTask =
            this.motionService.RetrieveByIdAsync(invalidMotionId);
         
        // Assert
        await Assert.ThrowsAsync<MotionValidationException>(() => retrieveByIdAsyncTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(It.IsAny<Guid>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}