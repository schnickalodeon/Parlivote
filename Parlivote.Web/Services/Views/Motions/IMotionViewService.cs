using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;

namespace Parlivote.Web.Services.Views.Motions;

public interface IMotionViewService
{
    Task<MotionView> AddAsync(MotionView motionView);
    Task<List<MotionView>> GetAllAsync();
    Task<List<MotionView>> GetAllWithMeetingAsync();
    Task<MotionView> GetActiveAsync();
    Task<MotionView> UpdateAsync(MotionView motionView);
    Task<MotionView> RemoveByIdAsync(Guid motionIdToDelete);
}