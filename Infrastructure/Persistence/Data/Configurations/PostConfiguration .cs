using Domain.Entities.Posts;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CreatedByUserId)
                   .IsRequired();

            builder.Property(p => p.GroupId)
                   .IsRequired();

            builder.HasOne<ApplicationUser>(p => p.CreatedByUser)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Group)
                   .WithMany()
                   .HasForeignKey(p => p.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
