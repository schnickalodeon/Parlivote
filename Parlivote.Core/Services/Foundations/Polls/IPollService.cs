using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Services.Foundations.Polls;

public interface IPollService
{
    Task<Poll> AddAsyncAsync(Poll poll);
}