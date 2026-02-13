using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupInvitaionSpecs
{
    public class GetInvitationByTokenSpecification: BaseSpecifications<GroupInvitation, int>
    {
        public GetInvitationByTokenSpecification(string token)
            : base(i => i.Token == token)
        {
            AddIncludes(i => i.Group);
        }
    }

}
