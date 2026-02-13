using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupModule
{
    public class GroupPermissionsDTO
    {
        public bool CanFollow { get; set; }
        public bool CanJoin { get; set; }
        public bool CanPost { get; set; }
        public bool CanCreatePoll { get; set; }
        public bool CanCreateEvent { get; set; }
        public bool CanDeletePost { get; set; }
        public bool CanModerate { get; set; }
        public bool CanEditGroup { get; set; }
        public bool CanDeleteGroup { get; set; }
    }

}
