using Domain.Entities.Engagement;
using Domain.Entities.Groups;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Domain.Entities.Stories;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.Contexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        #region DbSets
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupScore> GroupScores { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<MemoryReel> MemoryReels { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AIModeration> AIModerations { get; set; }

        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryView> StoryViews { get; set; }

        public DbSet<Album> Albums { get; set; }
        public DbSet<MediaItem> Media { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<PollVote> PollVotes { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessages { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        }
    }
}
