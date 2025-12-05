using Domain.Entities.Engagement;
using Domain.Entities.Groups;
using Domain.Entities.Posts;
using Domain.Entities.Stories;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicture { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        #region Relations
        public ICollection<GroupMember> GroupMembers { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<PollVote> PollVotes { get; set; }
        public ICollection<Recommendation> Recommendations { get; set; }
        public ICollection<StoryView> StoryViews { get; set; }
        public ICollection<GroupJoinRequest> GroupJoinRequests { get; set; }
        public ICollection<GroupFollower> GroupFollowers { get; set; }
        #endregion
    }
}
