using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Web.Models.Views.Polls;

namespace Parlivote.Web.Services.Views.Polls;

public interface IPollViewService
{
    Task<PollView> AddAsync(PollView pollView);
    Task<List<PollView>> GetAllAsync();
}