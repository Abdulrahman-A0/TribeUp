using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupInvitaionSpecs
{
    public class GetGroupInvitationsSpecification : BaseSpecifications<GroupInvitation, int>
    {
        public GetGroupInvitationsSpecification(int groupId, int page, int pageSize)
            : base(i => i.GroupId == groupId)
        {
            AddOrderByDescending(i => i.CreatedAt);
            ApplyPagination(page, pageSize);
        }
    }

}
