using Parlivote.Shared.Models.Motions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    Task<Motion> InsertMotionAsync(Motion poll);
    IQueryable<Motion> SelectAllMotions();
    Task<Motion> SelectMotionById(Guid pollId);
    Task<Motion> UpdateMotionAsync(Motion poll);
    Task<Motion> DeleteMotionAsync(Motion poll);
}