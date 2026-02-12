using Domain.Entities.Engagement;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Domain.Entities.Stories;
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
    public class Group : BaseEntity<int>
    {
        public string GroupName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? GroupProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AccessibilityType Accessibility { get; set; }

        public DateTime? LastMessageSentAt { get; set; }

        public GroupChatMessage? LastMessage { get; set; }
        public long? LastMessageId { get; set; }



        #region Navigation properties
        public virtual ICollection<GroupMembers> GroupMembers { get; set; } = new List<GroupMembers>();
        public virtual ICollection<GroupFollowers> GroupFollowers { get; set; } = new List<GroupFollowers>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<Poll> Polls { get; set; } = new List<Poll>();
        public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();
        public virtual ICollection<MemoryReel> MemoryReels { get; set; } = new List<MemoryReel>();
        public virtual GroupScore GroupScore { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

        #endregion
    }
}
