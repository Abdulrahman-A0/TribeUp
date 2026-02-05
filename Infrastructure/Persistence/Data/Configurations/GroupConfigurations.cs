using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    internal sealed class GroupConfigurations : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Description)
                .HasMaxLength(1000);

            builder.Property(g => g.GroupProfilePicture)
                .HasMaxLength(500);

            builder.Property(g => g.CreatedAt)
                .IsRequired();

            builder.Property(g => g.Accessibility)
                .IsRequired();

            builder.Property(g => g.LastMessageSentAt);

            builder.HasOne(g => g.LastMessage)
                .WithMany()
                .HasForeignKey(g => g.LastMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.GroupMembers)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.GroupScore)
                .WithOne(s => s.Group)
                .HasForeignKey<GroupScore>(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(g => g.LastMessageId);
            builder.HasIndex(g => g.LastMessageSentAt);
        }
    }
}
