using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Processing;

public interface IMotionProcessingService
{
    Task<Motion> AddAsync(Motion motion);
    IQueryable<Motion> RetrieveAll();
    Task<Motion> RetrieveActiveAsync();
    Task<Motion> RetrieveByIdAsync(Guid motionId);
    Task<Motion> ModifyAsync(Motion motion);
    Task<Motion> DeleteMotionById(Guid motionId);
}