using Microsoft.AspNetCore.Identity;

namespace Parlivote.Shared.Models.Identity;

public class Role : IdentityRole<Guid>
{
    public static Role Create(string id, string name)
    {
        return new Role()
        {
            Id = Guid.Parse(id),
            Name = name,
            NormalizedName = name.ToUpper(),
            ConcurrencyStamp = id
        };
    }
}