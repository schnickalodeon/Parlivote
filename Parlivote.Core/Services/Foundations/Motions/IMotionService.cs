using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Foundations.Motions;

public interface IMotionService
{
    Task<Motion> AddAsync(Motion poll);
    IQueryable<Motion> RetrieveAll();
}