using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Motions;

namespace Parlivote.Web.Services.Foundations.Motions;

public partial class MotionService : IMotionService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IApiBroker apiBroker;

    public MotionService(ILoggingBroker loggingBroker, IApiBroker apiBroker)
    {
        this.loggingBroker = loggingBroker;
        this.apiBroker = apiBroker;
    }

    public Task<Motion> AddAsync(Motion poll) =>
        TryCatch(async () => 
        {
            ValidateMotion(poll);
            return await this.apiBroker.PostMotionAsync(poll);
        });

    public Task<List<Motion>> RetrieveAllAsync() =>
        TryCatch(async () =>
        {
            return await this.apiBroker.GetAllMotionsAsync();
        });

    public Task<List<Motion>> RetrieveByApplicantId(Guid applicantId) =>
        TryCatch(async () => await this.apiBroker.GetByApplicantIdAsync(applicantId));

    public async Task<Motion> RetrieveActiveAsync()
    {
        return await this.apiBroker.GetActiveMotion();
    }

    public Task<Motion> ModifyAsync(Motion motion) =>
        TryCatch(async () =>
        {
            ValidateMotion(motion);

            Motion maybeMotion =
                await this.apiBroker.GetMotionById(motion.Id);

            ValidateStorageMotion(maybeMotion, motion.Id);

            return await this.apiBroker.PutMotionAsync(motion);
        });

    public Task<Motion> RemoveByIdAsync(Guid motionIdToDelete) =>
        TryCatch(async () =>
        {
            ValidateMotionId(motionIdToDelete);
            return await this.apiBroker.DeleteMotionById(motionIdToDelete);
        });
}