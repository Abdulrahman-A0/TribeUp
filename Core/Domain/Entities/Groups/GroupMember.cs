using Domain.Entities.Users;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Groups
{
    public class GroupMember : BaseEntity<int>
    {
        public bool IsCreator { get; set; }
        public RoleType Role { get; set; } = RoleType.Member;
        public DateTime JoinedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
