using Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal sealed class LikeConfigrations : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasOne(l => l.Post)
                  .WithMany(p => p.Likes)
                  .HasForeignKey(l => l.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(l => l.Comment)
                  .WithMany(c => c.Likes)
                  .HasForeignKey(l => l.CommentId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.User)
                  .WithMany(u => u.Likes)
                  .HasForeignKey(l => l.UserId)
                  .OnDelete(DeleteBehavior.Cascade);


            // CHECK constraint: either PostId OR CommentId
            builder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Likes_PostOrComment",
                    "(PostId IS NOT NULL AND CommentId IS NULL) OR " +
                    "(PostId IS NULL AND CommentId IS NOT NULL)"
                ));
        }
    }
}
