//using Domain.Entities.Groups;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace Service.Specifications.GroupJoinRequestSpecs
//{
//    public class GroupJoinRequestWithDetailsSpec : BaseSpecifications<GroupJoinRequest, int>
//    {
//        // Get request by ID with User and Group
//        public GroupJoinRequestWithDetailsSpec(int requestId)
//            : base(r => r.Id == requestId)
//        {
//            AddIncludes(r => r.User);
//            AddIncludes(r => r.Group);
//        }
//    }
//}
