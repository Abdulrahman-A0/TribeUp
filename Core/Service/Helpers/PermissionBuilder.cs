using Shared.DTOs.GroupModule;
using Shared.Enums;

namespace Service.Helper
{
    public static class PermissionBuilder
    {
        public static GroupPermissionsDTO Build(GroupRelationType relation)
        {
            return relation switch
            {
                GroupRelationType.None => new()
                {
                    CanFollow = true
                },

                GroupRelationType.Follower => new(),

                GroupRelationType.Member => new()
                {
                    CanPost = true,
                    CanCreatePoll = true,
                    CanCreateEvent = true
                },

                GroupRelationType.Admin => new()
                {
                    CanPost = true,
                    CanCreatePoll = true,
                    CanCreateEvent = true,
                    CanModerate = true,
                    CanDeletePost = true
                },

                GroupRelationType.Owner => new()
                {
                    CanPost = true,
                    CanCreatePoll = true,
                    CanCreateEvent = true,
                    CanModerate = true,
                    CanDeletePost = true,
                    CanEditGroup = true,
                    CanDeleteGroup = true
                }
            };
        }
    }

}
