using Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class PollVoteConfiguration : IEntityTypeConfiguration<PollVote>
    {
        public void Configure(EntityTypeBuilder<PollVote> builder)
        {
            builder.HasIndex(pv => new { pv.OptionId, pv.UserId }).IsUnique();

            builder.HasOne(pv => pv.Poll)
                .WithMany()
                .HasForeignKey(pv => pv.PollId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasOne(pv => pv.PollOption)
                .WithMany(o => o.PollVotes)
                .HasForeignKey(pv => pv.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pv => pv.User)
                .WithMany()
                .HasForeignKey(pv => pv.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
