using Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class EventParticipantConfiguration
        : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
            builder.HasIndex(x => new
            {
                x.EventId,
                x.UserId
            }).IsUnique();
           
            builder.HasOne(ep => ep.Event)
               .WithMany(e => e.Participants)
               .HasForeignKey(ep => ep.EventId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.User)
                .WithMany(u => u.EventParticipations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}