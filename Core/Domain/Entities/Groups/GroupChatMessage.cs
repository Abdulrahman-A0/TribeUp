using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Groups
{
    public class GroupChatMessage : BaseEntity<long>
    {
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public virtual Group Group { get; set; } = null!;
        public int GroupId { get; set; }


        public virtual ApplicationUser User { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
    }
}
