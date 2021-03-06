using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Foundations.Motions;

public partial class MotionService : IMotionService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IStorageBroker storageBroker;

    public MotionService(ILoggingBroker loggingBroker, IStorageBroker storageBroker)
    {
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
    }

    public Task<Motion> AddAsync(Motion poll) =>
        TryCatch(async () =>
        {
            ValidateMotion(poll);
            return await this.storageBroker.InsertMotionAsync(poll);
        });

    public IQueryable<Motion> RetrieveAll() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllMotions();
        });

    public Task<Motion> RetrieveByIdAsync(Guid motionId) =>
        TryCatch(async () =>
        {
            ValidateMotionId(motionId);
            return await this.storageBroker.SelectMotionById(motionId);
        });

    public Task<Motion> ModifyAsync(Motion motion) =>
        TryCatch(async () =>
        {
            ValidateMotion(motion);
            Motion maybeMotion =
                await this.storageBroker.SelectMotionById(motion.Id);

            ValidateStorageMotion(maybeMotion, motion.Id);

            return await this.storageBroker.UpdateMotionAsync(motion);
        });

    public Task<Motion> RemoveByIdAsync(Guid motionId) =>
        TryCatch(async () =>
        {
            ValidateMotionId(motionId);

            Motion motionToDelete =
                await this.storageBroker.SelectMotionById(motionId);

            return await this.storageBroker.DeleteMotionAsync(motionToDelete);
        });
}