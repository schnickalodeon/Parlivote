using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Foundations.Motions;

public interface IMotionService
{
    Task<Motion> AddAsync(Motion motion);
    IQueryable<Motion> RetrieveAll();
    Task<Motion> RetrieveByIdAsync(Guid motionId);
    Task<Motion> ModifyAsync(Motion motion);
    Task<Motion> RemoveByIdAsync(Guid motionId);
}