using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Processing;

public class MotionProcessingService : IMotionProcessingService
{
    private readonly IMotionService motionService;
    private readonly ILoggingBroker loggingBroker;

    public MotionProcessingService(
        IMotionService motionService, 
        ILoggingBroker loggingBroker)
    {
        this.motionService = motionService;
        this.loggingBroker = loggingBroker;
    }

    public async Task<Motion> AddAsync(Motion motion)
    {
        return await this.motionService.AddAsync(motion);
    }

    public IQueryable<Motion> RetrieveAll()
    {
        return this.motionService.RetrieveAll();
    }

    public Task<Motion> RetrieveActiveAsync()
    {
        IQueryable<Motion> allMotions =
            this.motionService.RetrieveAll();

        Expression<Func<Motion,bool>> activeMotionExpression =
            m => m.State == MotionState.Pending;

        Task<Motion> maybeActiveMotion =
            allMotions.SingleOrDefaultAsync(activeMotionExpression);

        return maybeActiveMotion;
    }
}