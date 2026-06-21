using Domain.Entities.VirtualRooms;
using Microsoft.EntityFrameworkCore;

namespace Service.Specifications.VirtualRoomSpecifications
{
    public class GetRoomByGroupIdSpecification : BaseSpecifications<VirtualRoom, int>
    {
        public GetRoomByGroupIdSpecification(int groupId)
            : base(vr => vr.GroupId == groupId)
        {
            AddThenIncludes(query => query
                .Include(vr => vr.Participants)
                .ThenInclude(p => p.User));
        }
    }
}
