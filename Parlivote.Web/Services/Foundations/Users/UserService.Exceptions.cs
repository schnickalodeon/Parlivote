using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace Parlivote.Web.Services.Foundations.Users;

public partial class UserService
{
    private delegate Task<User> ReturningUserFunction();
    private delegate Task<List<User>> ReturningUsersFunction();

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
        catch (HttpResponseNotFoundException httpResponseNotFoundException)
        {
            var notFoundUserException =
                new NotFoundUserException(httpResponseNotFoundException);

            throw CreateAndLogDependencyValidationException(notFoundUserException);
        }
        catch (InvalidUserException invalidUserException)
        {
            throw CreateAndLogValidationException(invalidUserException);
        }
        catch (HttpRequestException requestException)
        {
            var failedUserDependencyException =
                new FailedUserDependencyException(requestException);

            throw CreateAndLogCriticalDependencyException(failedUserDependencyException);
        }
        catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
        {
            var failedUserDependencyException =
                new FailedUserDependencyException(httpResponseUrlNotFoundException);

            throw CreateAndLogCriticalDependencyException(failedUserDependencyException);
        }
        catch (HttpResponseUnauthorizedException httpUnauthorizedException)
        {
            var failedUserDependencyException =
                new FailedUserDependencyException(httpUnauthorizedException);

            throw CreateAndLogCriticalDependencyException(failedUserDependencyException);
        }
        catch (HttpResponseConflictException httpConflictException)
        {
            throw CreateAndLogDependencyValidationException(httpConflictException);
        }
        catch (HttpResponseBadRequestException httpBadRequestException)
        {
            var invalidUserException =
                new InvalidUserException(
                    httpBadRequestException,
                    httpBadRequestException.Data);

            throw CreateAndLogDependencyValidationException(invalidUserException);
        }
        catch (HttpResponseInternalServerErrorException httpInternalServerErrorException)
        {
            throw CreateAndLogDependencyException(httpInternalServerErrorException);
        }
        catch (HttpResponseException httpResponseException)
        {
            var failedUserDependencyException =
                new FailedUserDependencyException(httpResponseException);

            throw CreateAndLogDependencyException(failedUserDependencyException);
        }
        catch (Exception exception)
        {
            var failedUserServiceException =
                new FailedUserServiceException(exception);

            throw CreateAndLogServiceException(failedUserServiceException);
        }
    }
    private async Task<List<User>> TryCatch(ReturningUsersFunction returningUsersFunction)
    {
        try
        {
            return await returningUsersFunction();
        }
        catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
        {
            throw CreateAndLogCriticalDependencyException(httpResponseUrlNotFoundException);
        }
        catch (HttpResponseUnauthorizedException httpUnauthorizedException)
        {
            throw CreateAndLogCriticalDependencyException(httpUnauthorizedException);
        }
        catch (HttpResponseInternalServerErrorException httpInternalServerErrorException)
        {
            throw CreateAndLogDependencyException(httpInternalServerErrorException);
        }
        catch (HttpResponseException httpResponseException)
        {
            var failedUserDependencyException =
                new FailedUserDependencyException(httpResponseException);

            throw CreateAndLogDependencyException(failedUserDependencyException);
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
        var pollValidationException =
            new UserValidationException(exception);

        this.loggingBroker.LogError(pollValidationException);

        return pollValidationException;
    }
    private UserDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var pollDependencyValidationException =
            new UserDependencyValidationException(exception);

        this.loggingBroker.LogError(pollDependencyValidationException);

        return pollDependencyValidationException;
    }
    private UserDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new UserDependencyException(exception);

        this.loggingBroker.LogError(pollDependencyException);

        return pollDependencyException;
    }
    private UserDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new UserDependencyException(exception);

        this.loggingBroker.LogCritical(pollDependencyException);

        return pollDependencyException;
    }
    private UserServiceException CreateAndLogServiceException(Xeption exception)
    {
        var pollServiceException =
            new UserServiceException(exception);

        this.loggingBroker.LogError(pollServiceException);

        return pollServiceException;
    }
}