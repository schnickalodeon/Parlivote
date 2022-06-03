using System;
using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private static void SeedRoles(ModelBuilder builder)
    {
        var roles = new Role[]
        {
            Role.Create("3b45d763-8154-410d-8e63-a07e19b6db5a", "chair"),
            Role.Create("f1f532a2-7121-442c-a745-121e0e87173e", "parliamentarian"),
            Role.Create("9e1f495f-8bfb-4b21-ac71-d54679e0e3c1", "applicant"),
        };

        builder.Entity<Role>().HasData(roles);
    }
}