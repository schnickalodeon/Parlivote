using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Shared.Models.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsAttendant { get; set; }

        public ICollection<Motion>? Motions { get; set; }
    }
}
