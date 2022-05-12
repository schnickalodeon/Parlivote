using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.VoteValues;

namespace Parlivote.Shared.Models.Votes
{
    public class Vote
    {
        public Guid Id { get; set; }
        
        public Guid MotionId { get; set; }
        public Motion Motion { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public VoteValue Value { get; set; }
    }
}
