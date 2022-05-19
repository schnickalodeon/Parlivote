using System;
using EFxceptions;
using EFxceptions.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker : EFxceptionsIdentityContext<User, Role, Guid>, IStorageBroker
{
    private readonly IConfiguration configuration;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
        //this.Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SetMeetingReference(modelBuilder);
        AddMeetingAttendanceReference(modelBuilder);
        //ApplyRefreshTokenAnnotations(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = this.configuration
            .GetConnectionString(name: "DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);
    }

    public override void Dispose() { }
}