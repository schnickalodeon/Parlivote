using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Shared.Models.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        public bool IsLoggedIn { get; set; }
        public bool IsAttendant { get; set; }
    }
}
