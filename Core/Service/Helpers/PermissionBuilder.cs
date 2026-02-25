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
            };
        }
    }

}
