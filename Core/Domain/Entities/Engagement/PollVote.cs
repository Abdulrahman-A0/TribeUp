using Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class PollVote : BaseEntity<int>
    {
        public DateTime VotedAt { get; set; } = DateTime.Now;

        #region Relations
        public int OptionId { get; set; }
        [ForeignKey(nameof(OptionId))]
        public virtual PollOption PollOption { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
