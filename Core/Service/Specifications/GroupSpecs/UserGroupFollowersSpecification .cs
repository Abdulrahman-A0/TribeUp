using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class UserGroupFollowersSpecification: BaseSpecifications<GroupFollowers, int>
    {
        public UserGroupFollowersSpecification(string userId)
            : base(f => f.UserId == userId)
        {
        }
    }

}
