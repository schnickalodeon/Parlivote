using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;

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
}