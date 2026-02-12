using Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal sealed class TagConfigrations : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasOne(t => t.Post)
                      .WithMany(p => p.Tags)
                      .HasForeignKey(t => t.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Comment)
                  .WithMany(c => c.Tags)
                  .HasForeignKey(t => t.CommentId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Tags_PostOrComment",
                    "(PostId IS NOT NULL AND CommentId IS NULL) OR " +
                    "(PostId IS NULL AND CommentId IS NOT NULL)"
                ));
        }
    }
}
