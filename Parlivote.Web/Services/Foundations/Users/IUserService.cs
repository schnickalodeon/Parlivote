﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Web.Services.Foundations.Users;

public interface IUserService
{
    Task<List<User>> RetrieveAllAsync();
}