using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Web.Services.Foundations.Motions;

public interface IMotionService
{
    Task<Motion> AddAsync(Motion motion);
    Task<List<Motion>> RetrieveAllAsync();
    Task<Motion> RetrieveActiveAsync();
    Task<Motion> ModifyAsync(Motion motion);
    Task<Motion> RemoveByIdAsync(Guid motionIdToDelete);
}