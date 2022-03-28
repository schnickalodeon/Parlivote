using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Shared.Models.Motions.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Parlivote.Core.Tests.Unit;

public static class Tests
{
    public static int GetRandomNumber() =>
        new IntRange(min: 2, max: 10).GetValue();

    public static string GetRandomString() =>
        new MnemonicString(wordCount: GetRandomNumber()).GetValue();

    public static DateTimeOffset GetRandomDateTimeOffset() =>
        new DateTimeRange(earliestDate: new DateTime()).GetValue();

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

    public static void VerifyExceptionLogged(Mock<ILoggingBroker> loggingBrokerMock, Xeption expectedException)
    {
        loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(expectedException))),
            Times.Once);
    }
    public static void VerifyCriticalExceptionLogged(Mock<ILoggingBroker> loggingBrokerMock, Xeption expectedException)
    { 
        loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(expectedException))),
            Times.Once);
    }
}