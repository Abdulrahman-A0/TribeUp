using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal sealed class GroupInvitationConfiguration : IEntityTypeConfiguration<GroupInvitation>
    {
        public void Configure(EntityTypeBuilder<GroupInvitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Token)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasIndex(i => i.Token)
                   .IsUnique();

            builder.Property(i => i.MaxUses)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.HasOne(i => i.Group)
                   .WithMany(g => g.Invitations)
                   .HasForeignKey(i => i.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.User)
                   .WithMany()
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_GroupInvitation_Usage",
                        "MaxUses >= 1 AND UsedCount <= MaxUses"
                    ));
        }
    }
}
