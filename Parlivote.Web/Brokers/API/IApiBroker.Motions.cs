using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<Motion> PostMotionAsync(Motion poll);
    Task<List<Motion>>GetAllMotionsAsync();
    Task<Motion> GetMotionById(Guid pollId);
    Task<Motion> PutMotionAsync(Motion poll);
    Task<Motion> DeleteMotionById(Guid pollId);
}