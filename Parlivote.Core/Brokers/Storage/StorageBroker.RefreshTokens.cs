using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public async Task<RefreshToken> SelectRefreshTokenByToken(string token)
    {
        await using var broker = new StorageBroker(this.configuration);
        return await broker.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
    }

    public async Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken token)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<RefreshToken> updatedEntityEntry =
            broker.RefreshTokens.Update(token);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }

    public async Task<RefreshToken> InsertRefreshTokenAsync(RefreshToken token)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<RefreshToken> updatedEntityEntry =
            broker.RefreshTokens.Add(token);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }
}