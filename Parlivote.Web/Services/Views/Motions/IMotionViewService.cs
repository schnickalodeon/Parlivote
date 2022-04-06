using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;

namespace Parlivote.Web.Services.Views.Motions;

public interface IMotionViewService
{
    Task<MotionView> AddAsync(MotionView pollView);
    Task<List<MotionView>> GetAllAsync();
    Task<MotionView> GetActiveAsync();
    Task<MotionView> UpdateAsync(MotionView motionView);
}