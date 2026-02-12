//using Domain.Entities.Groups;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Service.Specifications.GroupJoinRequestSpecs
//{
//    public class AllGroupJoinRequestsByUserSpec : BaseSpecifications<GroupJoinRequest, int>
//    {
//        // Get all requests by a user with Group details
//        public AllGroupJoinRequestsByUserSpec(string userId)
//            : base(r => r.UserId == userId)
//        {
//            AddIncludes(r => r.Group);
//        }
//    }
//}
