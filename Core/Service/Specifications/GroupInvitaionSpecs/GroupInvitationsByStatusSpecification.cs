using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupInvitaionSpecs
{
    public class GroupInvitationsByStatusSpecification : BaseSpecifications<GroupInvitation, int>
    {
        public GroupInvitationsByStatusSpecification(int groupId, bool isRevoked)
            : base(i => i.GroupId == groupId && i.IsRevoked == isRevoked)
        {
        }
    }
}
