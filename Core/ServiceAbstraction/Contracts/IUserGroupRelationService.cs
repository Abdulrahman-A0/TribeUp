using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IUserGroupRelationService
    {
        Task InitializeAsync(string userId);

        GroupRelationType GetRelation(int groupId);

        bool IsAdmin(int groupId);

        bool IsOwner(int groupId);

        bool IsMember(int groupId);

        bool IsFollower(int groupId);
        
        bool IsNone(int groupId);
    }

}
