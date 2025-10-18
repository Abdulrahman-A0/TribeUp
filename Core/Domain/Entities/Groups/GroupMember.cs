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
        public RoleType Role { get; set; } = RoleType.Member;
        public DateTime JoinedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public virtual Group Group { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        #endregion
    }
}
