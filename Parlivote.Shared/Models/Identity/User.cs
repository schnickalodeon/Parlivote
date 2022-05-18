using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Shared.Models.Identity
{
    public class User : IdentityUser<Guid>
    {
        public List<Meeting>? Meetings { get; set; }
    }
}
