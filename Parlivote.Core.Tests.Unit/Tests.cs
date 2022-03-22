using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Shared.Models.Polls.Exceptions;
using Xeptions;

namespace Parlivote.Core.Tests.Unit;

public static class Tests
{
    public static SqlException GetSqlException() =>
        (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

    public static Expression<Func<Exception, bool>> SameExceptionAs(Xeption expectedException)
    {
        return actualException =>
            actualException.Message == expectedException.Message &&
            actualException.InnerException.Message == expectedException.InnerException.Message &&
            (actualException.InnerException as Xeption).DataEquals(expectedException.InnerException.Data) &&
            (expectedException.InnerException as Xeption).DataEquals(actualException.InnerException.Data);
    }

    public static void VerifyCriticalExceptionLogged(Mock<ILoggingBroker> loggingBrokerMock, Xeption expectedException)
    {
        loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(expectedException))),
            Times.Once);
    }
}