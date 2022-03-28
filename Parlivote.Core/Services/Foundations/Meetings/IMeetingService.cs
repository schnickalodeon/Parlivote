﻿using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Services.Foundations.Meetings;

public interface IMeetingService
{
    Task<Meeting> AddAsync(Meeting meeting);
}