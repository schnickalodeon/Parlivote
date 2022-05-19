using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using Xeptions;

namespace Parlivote.Core.Services.Foundations.Users;

public partial class UserService
{
    private delegate Task<User> ReturningUserFunction();
    private delegate IQueryable<User> ReturningUsersFunction();

    private async Task<User> TryCatch(ReturningUserFunction returningUserFunction)
    {
        try
        {
            return await returningUserFunction();
        }
        catch (NullUserException nullUserException)
        {
            throw CreateAndLogValidationException(nullUserException);
        }
        catch (InvalidUserException invalidUserException)
        {
            throw CreateAndLogValidationException(invalidUserException);
        }
        catch (NotFoundUserException notFoundUserException)
        {
            throw CreateAndLogValidationException(notFoundUserException);
        }
        catch (SqlException sqlException)
        {
            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedUserStorageException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsUserException =
                new AlreadyExistsUserException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsUserException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            var lockedUserException =
                new LockedUserException(dbUpdateConcurrencyException);

            throw CreateAndLogDependencyValidationException(lockedUserException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            var failedUserStorageException =
                new FailedUserStorageException(dbUpdateException);

            throw CreateAndLogDependencyException(failedUserStorageException);
        }
        catch (Exception exception)
        {
            var failedUserServiceException =
                new FailedUserServiceException(exception);

            throw CreateAndLogServiceException(failedUserServiceException);
        }
    }

    private IQueryable<User> TryCatch(ReturningUsersFunction returningUsersFunction)
    {
        try
        {
            return returningUsersFunction();
        }
        catch (NotFoundUserException notFoundUserException)
        {
            throw CreateAndLogValidationException(notFoundUserException);
        }
        catch (SqlException sqlException)
        {
            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedUserStorageException);
        }
        catch (Exception exception)
        {
            var failedUserServiceException =
                new FailedUserServiceException(exception);

            throw CreateAndLogServiceException(failedUserServiceException);
        }
    }
        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var divisionManagerValidationException =
                new UserValidationException(exception);

            this.loggingBroker.LogError(divisionManagerValidationException);

            return divisionManagerValidationException;
        }

        private UserServiceException CreateAndLogServiceException(Xeption exception)
        {
            var divisionManagerServiceException
                = new UserServiceException(exception);

            this.loggingBroker.LogError(divisionManagerServiceException);

            return divisionManagerServiceException;
        }

        private UserDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var divisionManagerDependencyException =
                new UserDependencyException(exception);

            this.loggingBroker.LogCritical(divisionManagerDependencyException);

            return divisionManagerDependencyException;
        }

        private UserDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var divisionManagerDependencyValidationException =
                new UserDependencyValidationException(exception);

            this.loggingBroker.LogError(divisionManagerDependencyValidationException);

            return divisionManagerDependencyValidationException;
        }

        private UserDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var divisionManagerDependencyException =
                new UserDependencyException(exception);

            this.loggingBroker.LogError(divisionManagerDependencyException);

            return divisionManagerDependencyException;
        }
}