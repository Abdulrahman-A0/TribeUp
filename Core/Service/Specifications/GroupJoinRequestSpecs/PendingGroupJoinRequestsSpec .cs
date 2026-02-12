//using Domain.Entities.Groups;
//using Shared.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace Service.Specifications.GroupJoinRequestSpecs
//{
//    public class PendingGroupJoinRequestsSpec : BaseSpecifications<GroupJoinRequest, int>
//    {
//        // Gets all pending requests for a specific group
//        public PendingGroupJoinRequestsSpec(int groupId)
//            : base(r => r.GroupId == groupId && r.Status == JoinRequestStatus.Pending)
//        {
//            AddIncludes(r => r.User);
//            AddIncludes(r => r.Group);
//        }
//    }
//}
