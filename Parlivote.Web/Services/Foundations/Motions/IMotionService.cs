using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Web.Services.Foundations.Motions;

public interface IMotionService
{
    Task<Motion> AddAsync(Motion poll);
    Task<List<Motion>> RetrieveAllAsync();
    Task<Motion> RetrieveActiveAsync();
}