using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class PollOption : BaseEntity<int>
    {
        public string OptionText { get; set; } = string.Empty;

        #region Relations
        public int PollId { get; set; }
        [ForeignKey(nameof(PollId))]
        public virtual Poll Poll { get; set; }

        public virtual ICollection<PollVote> PollVotes { get; set; } = new List<PollVote>();
        #endregion
    }
}
