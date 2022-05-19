using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Shared.Models.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        [JsonIgnore]
        public List<Meeting>? Meetings { get; set; }
    }
}
