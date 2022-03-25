using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Services.Foundations.Polls;

public interface IPollService
{
    Task<Poll> AddAsync(Poll poll);
    IQueryable<Poll> RetrieveAll();
}