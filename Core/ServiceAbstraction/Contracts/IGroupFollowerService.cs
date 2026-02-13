using Shared.DTOs.GroupFollowerModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupFollowerService
    {
        public Task<FollowResultDTO> ToggleFollow(int groupId, string userId);
    }
}
