﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Motions;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly Mock<IApiBroker> apiBrokerMock;
    private readonly IMotionService pollService;
    public MotionServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.apiBrokerMock = new Mock<IApiBroker>();

        this.pollService = new MotionService(
            this.loggingBrokerMock.Object,
            this.apiBrokerMock.Object);
    }
    private static Motion GetRandomMotion() =>
        GetMotionFiller().Create();

    private static IQueryable<Motion> GetRandomMotions(MotionState state = MotionState.Unset)
    {
        return GetMotionFiller(state)
            .Create(count: Tests.GetRandomNumber())
            .AsQueryable();
    }

    private static Filler<Motion> GetMotionFiller(MotionState state = MotionState.Unset)
    {
        var filler = new Filler<Motion>();

        if (state == MotionState.Unset)
        {
            filler.Setup().OnType<Meeting>().IgnoreIt();
        }
        else
        {
            filler.Setup()
                .OnType<MotionState>().Use(state)
                .OnType<Meeting>().IgnoreIt();
        }
        
        return filler;
    }

    public static TheoryData DependencyException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var httpResponseException =
            new HttpResponseException(
                httpResponseMessage: responseMessage,
                message: randomMessage);

        var httpResponseInternalServerErrorException =
            new HttpResponseInternalServerErrorException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            httpResponseException,
            httpResponseInternalServerErrorException
        };
    }

    public static TheoryData CriticalDependencyException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var urlNotFounException =
            new HttpResponseUrlNotFoundException(
                responseMessage: responseMessage,
                message: randomMessage);

        var unauthorizedException =
            new HttpResponseUnauthorizedException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            urlNotFounException,
            unauthorizedException
        };
    }

    public static TheoryData DependencyValidationException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var badRequestException =
            new HttpResponseBadRequestException(
                responseMessage: responseMessage,
                message: randomMessage);

        var conflictException =
            new HttpResponseConflictException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            badRequestException,
            conflictException
        };
    }
}