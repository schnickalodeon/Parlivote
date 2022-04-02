using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Web.Services.Foundations.Meetings;

public interface IMeetingService
{
    Task<Meeting> AddAsync(Meeting meeting);
    Task<List<Meeting>> RetrieveAllAsync();
    Task<List<Meeting>> RetrieveAllWithMotionsAsync();
    Task<Meeting> DeleteByIdAsync(Guid meetingId);
}