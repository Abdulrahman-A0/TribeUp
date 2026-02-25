using Shared.DTOs.GroupModule;
using Shared.Enums;

namespace Service.Helper
{
    public static class PermissionBuilder
    {
        public static PostPermissionsDTO Build(GroupRelationType relation)
        {
            return relation switch
            {
                GroupRelationType.None => new(),
                GroupRelationType.Follower => new(),
                GroupRelationType.Member => new(),

                GroupRelationType.Admin => new()
                {
                    CanModerate = true,
                    CanDelete = true
                },

                GroupRelationType.Owner => new()
                {
                    CanModerate = true,
                    CanDelete = true,
                }
                //_ => throw new ArgumentOutOfRangeException(nameof(relation), relation, null)
            };
        }
    }

}
