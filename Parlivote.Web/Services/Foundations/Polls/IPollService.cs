using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Web.Services.Foundations.Polls;

public interface IPollService
{
    Task<Poll> AddAsync(Poll poll);
}