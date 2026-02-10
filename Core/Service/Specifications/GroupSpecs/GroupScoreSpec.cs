using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class GroupScoreSpec : BaseSpecifications<Group, int>
    {
        public GroupScoreSpec(int groupId)
            : base(g => g.Id == groupId)
        {
            AddIncludes(g => g.GroupScore);
        }
    }
}
