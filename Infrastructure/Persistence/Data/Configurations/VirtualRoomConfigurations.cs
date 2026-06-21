using Domain.Entities.VirtualRooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal sealed class VirtualRoomConfigurations : IEntityTypeConfiguration<VirtualRoom>
    {
        public void Configure(EntityTypeBuilder<VirtualRoom> builder)
        {
            builder.HasOne(vr => vr.Group)
                .WithOne()
                .HasForeignKey<VirtualRoom>(vr => vr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal sealed class RoomParticipantConfigurations : IEntityTypeConfiguration<RoomParticipant>
    {
        public void Configure(EntityTypeBuilder<RoomParticipant> builder)
        {
            builder.HasIndex(rp => new { rp.VirtualRoomId, rp.UserId })
                .IsUnique();
        }
    }
}
